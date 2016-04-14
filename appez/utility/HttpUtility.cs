using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.model.fileupload;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace appez.utility
{
    /// <summary>
    /// Utility class that enables processing of
    /// HTTP operation. Currently supports HTTP GET and POST operations. Also
    /// supports file upload on a remote server
    /// </summary>
    public class HttpUtility
    {
        #region variables
        private string requestURL = null;
        private string requestVerb = null;
        private string requestBody = null;
        private string requestHeader = null;
        private string requestFileUploadInfo = null;
        private string requestContentEncoding = null;
        private string MimeType = "application/octet-stream";
        private string encoding = "binary";
        private string requestFileToSaveName = null;
        private string fileToSaveLocation = null;

        string lineStart = "--";
        string lineEnd = Environment.NewLine;
        private string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

        private Dictionary<string, string> headerMap = new Dictionary<string, string>();
        public const string METHOD_POST = "POST";
        public const string METHOD_GET = "GET";
        List<FileInfoBean> fileMetaInfoList;

        private bool bCreateFileDump = false;
        private SmartNetworkListener smartNetworkListener = null;
        private static CookieContainer cookieContainer = null;
        private HttpWebRequest request = null;
        #endregion

        public HttpUtility(SmartNetworkListener listener)
        {
            this.smartNetworkListener = listener;
            // create singleton cookie container so that cookies recieved 
            // in current request can be sent in next request. 
            if (null == cookieContainer)
            {
                cookieContainer = new CookieContainer();
            }
        }
        /// <summary>
        /// Performs HTTP action based on the request data
        /// </summary>
        /// <param name="requestdata">Http request parameter.</param>
        /// <param name="createFile">Indicates whether or not a temporary file needs to be
        /// created for dumping data from HTTP action</param>
        public void PerformAsyncRequest(string requestdata, bool createFile)
        {
            try
            {
                this.bCreateFileDump = createFile;
                this.ParseDataCallback(requestdata);
            
                this.request = (HttpWebRequest)WebRequest.Create(this.requestURL);

                this.request.CookieContainer = cookieContainer;

                if (this.requestVerb != null)
                {
                    this.request.Method = this.requestVerb;
                    if ((this.requestVerb == SmartConstants.HTTP_REQUEST_TYPE_GET))
                    {
                        if (this.headerMap.Count > 0)
                        {
                            this.AddHeaders();
                        }

                        this.request.BeginGetResponse(new AsyncCallback(this.GetResponseCallback), this.request);

                    }
                    else if(this.requestVerb == SmartConstants.HTTP_REQUEST_TYPE_POST)
                    {

                        this.request.ContentType = this.requestContentEncoding;
                        if ((this.requestFileUploadInfo != null) && (this.requestFileUploadInfo.Length > 0))
                        {
                            this.request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                        }

                        if (this.headerMap.Count > 0)
                        {
                            this.AddHeaders();
                        }
                        this.request.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCallback), this.request);

                    }
                    else if (this.requestVerb == SmartConstants.HTTP_REQUEST_TYPE_PUT)
                    {
                        this.request.ContentType = this.requestContentEncoding;
                        
                        if (this.headerMap.Count > 0)
                        {
                            this.AddHeaders();
                        }
                        this.request.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCallback), this.request);
                    }
                    else if (this.requestVerb == SmartConstants.HTTP_REQUEST_TYPE_DELETE)
                    {
                        this.request.ContentType = this.requestContentEncoding;

                        if (this.headerMap.Count > 0)
                        {
                            this.AddHeaders();
                        }
                        this.request.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCallback), this.request);
                    }
                }
            }
            catch (UriFormatException uriEx)
            {
                smartNetworkListener.OnErrorHttpOperation(ExceptionTypes.MALFORMED_URL_EXCEPTION, uriEx.Message.ToString());
            }
            catch (Exception ex)
            {
                smartNetworkListener.OnErrorHttpOperation(ExceptionTypes.UNKNOWN_NETWORK_EXCEPTION, ex.Message.ToString());
            }

        }
        /// <summary>
        /// Add header to http request
        /// </summary>
        private void AddHeaders()
        {
            foreach (KeyValuePair<string, string> pair in this.headerMap)
            {
                switch (pair.Key)
                {
                    case "Accept":
                        if (pair.Value.Length > 0)
                        {
                            this.request.Accept = pair.Value;
                        }
                        break;
                    case "Content-Length":
                        if (pair.Value.Length > 0)
                        {
                            this.request.ContentLength = long.Parse(pair.Value);
                        }
                        break;
                    case "Content-Type":
                        if (this.requestVerb == "POST")
                        {
                            if (pair.Value.Length > 0)
                            {
                                this.request.ContentType = pair.Value;
                            }
                        }
                        break;
                    case "User-Agent":
                        if (pair.Value.Length > 0)
                        {
                            this.request.UserAgent = pair.Value;
                        }
                        break;

                    default:

                        try
                        {
                            if (pair.Value.Length > 0)
                            {
                                this.request.Headers[pair.Key] = pair.Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                smartNetworkListener.OnErrorHttpOperation(ExceptionTypes.HTTP_PROCESSING_EXCEPTION, ex.Message.ToString());
                            });
                        }
                        break;
                }


            }
        }
        /// <summary>
        /// Callback method of http request.
        /// </summary>
        /// <param name="asynchronousResult">http request result</param>
        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            if (this.requestBody != null)
            {
                HttpWebRequest asyncState = (HttpWebRequest)asynchronousResult.AsyncState;
                using (Stream stream = asyncState.EndGetRequestStream(asynchronousResult))
                {
                    if ((this.requestFileUploadInfo != null) && (this.requestFileUploadInfo.Length > 0))
                    {
                        // Sent data as multipart/form-data
                        fileMetaInfoList = ParseFileMetaInfo(JArray.Parse(this.requestFileUploadInfo));

                        // Add post parameter to request.
                        AddPostParameter(stream);

                        // Add File data into request.
                        AddFileMetaInfo(stream);
                        
                        
                        byte[] endRequest = System.Text.Encoding.UTF8.GetBytes(lineEnd + lineStart + boundary + lineStart + lineEnd);
                        stream.Write(endRequest, 0, endRequest.Length);

                    }
                    else
                    {
                        // sent data as application/x-www-form-urlencoded
                        string requestBody = this.requestBody;
                        byte[] bytes = Encoding.UTF8.GetBytes(requestBody);
                        stream.Write(bytes, 0, requestBody.Length);

                    }
                }
                asyncState.BeginGetResponse(new AsyncCallback(this.GetResponseCallback), asyncState);
            }
            else
            {
                throw new MobiletException(ExceptionTypes.HTTP_PROCESSING_EXCEPTION);
            }
        }

        /// <summary>
        /// Call back method of http post request
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest asyncState = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)asyncState.EndGetResponse(asynchronousResult);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        
                        string responseString = null;
                        string responseWithHeaderStr = null;
                        responseString = new StreamReader(responseStream).ReadToEnd();
                        byte[] responseData = StreamToByteArray(responseStream);
						if (CheckIfImageUrl())
			            {
			                responseString = Convert.ToBase64String(responseData);
			                responseString = responseString.Replace("\\n", "");
			            }
			              
                        if (this.bCreateFileDump)
                        {
                            CreateFileDump(responseStream);
                            responseWithHeaderStr = AddHeadersToResponse(response, this.fileToSaveLocation);
                        }
                        else
                        {
                            WebHeaderCollection responseHeaders = response.Headers;
                            
                            if ((responseHeaders != null) && (responseHeaders.Count > 0))
                            {
                                // If the response contains headers, then a JSON needs
                                // to be created in the response that contains server
                                // response in JSON format and also the response headers
                                responseWithHeaderStr = AddHeadersToResponse(response, responseString);
                                
                            }

                        }
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            this.smartNetworkListener.OnSuccessHttpOperation(responseWithHeaderStr);
                        });
                    }
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                      {
                           smartNetworkListener.OnErrorHttpOperation(ExceptionTypes.HTTP_PROCESSING_EXCEPTION, "");
                       });
                }
                response.Close();
            }
            catch (WebException we)
            {
                string responseString = "";
                if (we.Response != null)
                {
                    // cast to HttpWebResponse 
                    HttpWebResponse weResp = we.Response as HttpWebResponse;
                    using (Stream responseStream = weResp.GetResponseStream())
                    {
                        responseString = new StreamReader(responseStream).ReadToEnd();
                        responseStream.Close();
                        responseString = GetJSONEncodedData(responseString, true);
                    }

                }
                if (responseString == "")
                {
                    responseString = Convert.ToBase64String(Encoding.UTF8.GetBytes(we.Message.ToString()), 0, responseString.Length);
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                       {
                           smartNetworkListener.OnErrorHttpOperation(ExceptionTypes.HTTP_PROCESSING_EXCEPTION, we.Message);
                       });
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                       {
                           smartNetworkListener.OnErrorHttpOperation(ExceptionTypes.HTTP_PROCESSING_EXCEPTION, ex.Message);
                       });
            }
        }

       
        /// <summary>
        /// Parses request data to extract HTTP request parameters
        /// </summary>
        /// <param name="dataCallback">Request data containing parameters for performing HTTP
        /// action</param>
        private void ParseDataCallback(string dataCallback)
        {
            
            try
            {
                JObject httpRequestData = JObject.Parse(dataCallback);
                JToken tempToken = null;
                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_URL, out tempToken))
                {
                    this.requestURL = tempToken.ToString();
                }
                else
                {
                    this.requestURL = "";
                }

                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_METHOD, out tempToken))
                {
                    this.requestVerb = tempToken.ToString();
                }
                else
                {
                    this.requestVerb = "";
                }

                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_POST_BODY, out tempToken))
                {
                    this.requestBody = tempToken.ToString();
                }
                else
                {
                    this.requestBody = "";
                }

                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_HEADER_INFO, out tempToken))
                {
                    this.requestHeader = tempToken.ToString();
                    ParseRequestHeader(this.requestHeader);
                }
                else
                {
                    this.requestHeader = "";
                }

                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_FILE_INFO, out tempToken))
                {
                    this.requestFileUploadInfo = tempToken.ToString();
                }
                else
                {
                    this.requestFileUploadInfo = "";
                }

                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_CONTENT_TYPE, out tempToken))
                {
                    this.requestContentEncoding = tempToken.ToString();
                }
                else
                {
                    this.requestContentEncoding = "";
                }
                if (httpRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_REQ_FILE_TO_SAVE_NAME, out tempToken))
                {
                    this.requestFileToSaveName = tempToken.ToString(); 
                }
                else
                {
                    this.requestFileToSaveName = "";
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }		

        }

        /// <summary>
        /// Parses the request header string to extract information regarding the
        /// request headers.
        /// </summary>
        /// <param name="reqHeader">String containing all the request headers associated with
        /// the HTTP request</param>
        private void ParseRequestHeader(string reqHeader)
        {

            try
            {
                if (reqHeader != null && reqHeader != "")
                {
                    JArray reqHeadersArray = JArray.Parse(reqHeader);
                    if (reqHeadersArray.Count > 0)
                    {
                        
                        int totalReqHeaders = reqHeadersArray.Count;
                        foreach(var currentHeader in reqHeadersArray) 
                        {   
                            string headerKey = currentHeader.Value<string>(CommMessageConstants.MMI_REQUEST_PROP_HTTP_HEADER_KEY);
                            string headerValue =currentHeader.Value<string>(CommMessageConstants.MMI_REQUEST_PROP_HTTP_HEADER_VALUE);
                            this.headerMap.Add(headerKey, headerValue);
                        }
                    }
                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }

        }

        /// <summary>
        /// Parse File Meta info jarray to extract information regarding file.
        /// </summary>
        /// <param name="fileMetaInfoArr">JArray containing file meta information</param>
        /// <returns></returns>
        private List<FileInfoBean> ParseFileMetaInfo(JArray fileMetaInfoArr)
        {
            List<FileInfoBean> fileMetaInfoList = new List<FileInfoBean>();

            foreach (object item in fileMetaInfoArr)
            {
                FileInfoBean fileMetaInfo = JsonConvert.DeserializeObject<FileInfoBean>(item.ToString());
                fileMetaInfoList.Add(fileMetaInfo);
            }

            return fileMetaInfoList;
        }

        /// <summary>
        /// Adds header to http response.
        /// </summary>
        /// <param name="response">http response</param>
        /// <param name="serverResponseString">server response</param>
        /// <returns></returns>
        private string AddHeadersToResponse(HttpWebResponse response, string serverResponseString)
        {
            string responseWithHeaderStr = null;
            try
            {

                String responseString = serverResponseString;
                //retrieve header from response
                WebHeaderCollection responseHeaders = response.Headers;
                // create new json object and json array.
                
                JObject responseWithHeader = new JObject();
                JArray responseHeadersArray = new JArray();
                // adding all header to json array.
                foreach (string key in responseHeaders.AllKeys)
                {
                    string value = responseHeaders[key];
                    JObject currentHeader = new JObject();
                    currentHeader[CommMessageConstants.MMI_RESPONSE_PROP_HTTP_HEADER_NAME] = key;
                    currentHeader[CommMessageConstants.MMI_RESPONSE_PROP_HTTP_HEADER_VALUE] = value;
                    responseHeadersArray.Add(currentHeader);

                }
                // convert response string to json.
                responseString = GetJSONEncodedData(responseString, false);
                // create json to be send. 
                responseWithHeader.Add(new JProperty(CommMessageConstants.MMI_RESPONSE_PROP_HTTP_RESPONSE, responseString));
                responseWithHeader.Add(new JProperty(CommMessageConstants.MMI_RESPONSE_PROP_HTTP_RESPONSE_HEADERS, responseHeadersArray));

                responseWithHeaderStr = responseWithHeader.ToString();
                response.Close();
            }

            catch (IOException)
            {
                throw new MobiletException(ExceptionTypes.HTTP_PROCESSING_EXCEPTION);
            }
            catch (Exception)
            {
                throw new MobiletException(ExceptionTypes.HTTP_PROCESSING_EXCEPTION);
            }
            return responseWithHeaderStr;
        }

        /// <summary>
        /// convert response to json and return encoded string.
        /// </summary>
        /// <param name="response">response</param>
        /// <param name="shouldBase64Encode">bool</param>
        /// <returns>encoded string</returns>
        private string GetJSONEncodedData(string response, bool shouldBase64Encode)
        {
            string responseString = response;
            try
            {
                // Check if the server response is in XML or JSON
                if ((responseString.Contains(SmartConstants.RESPONSE_TYPE_XML))
                        || (responseString.StartsWith(SmartConstants.RESPONSE_TYPE_XML_START_SYMBOL) && responseString.EndsWith(SmartConstants.RESPONSE_TYPE_XML_END_SYMBOL)))
                {
                    
                    XmlNodeConverter converter = new XmlNodeConverter();
                    XDocument doc = XDocument.Parse(responseString);
                    responseString = JsonConvert.SerializeObject(doc, Formatting.None, converter);

                }
                else if ((responseString.StartsWith(SmartConstants.JSON_RESPONSE_START_IDENTIFIER_OBJECT) && responseString.EndsWith(SmartConstants.JSON_RESPONSE_END_IDENTIFIER_OBJECT))
                      || (responseString.StartsWith(SmartConstants.JSON_RESPONSE_START_IDENTIFIER_ARRAY) && responseString.EndsWith(SmartConstants.JSON_RESPONSE_END_IDENTIFIER_ARRAY)))
                {
                    // do nothing here as we want the JSON data only
                }
                else
                {
                    //else convert the response string into Base64 encoded string and send it to Javascript layer 

                }
                if (shouldBase64Encode)
                {
                    responseString = Convert.ToBase64String(Encoding.UTF8.GetBytes(responseString), 0, responseString.Length);
                    responseString = responseString.Replace("\\n", "");
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            catch (Exception)
            {
                throw new MobiletException(ExceptionTypes.HTTP_PROCESSING_EXCEPTION);
            }
            return responseString;
        }

        /// <summary>
        /// Add post parameter to request stream.
        /// </summary>
        /// <param name="stream">request stream</param>
        private void AddPostParameter(Stream stream)
        {
            if (this.requestBody != null && this.requestBody.Length > 0)
            {
                byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes(lineStart + boundary + lineEnd);
                // here we assume that the request has properties
                // separated by '&' separator for Multipartentity
                string[] postRequestParams = this.requestBody.Split('&');
                int totalParams = postRequestParams.Length;
                for (int param = 0; param < totalParams; param++)
                {
                    string[] paramKeyValue = postRequestParams[param].Split('=');
                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"" + lineEnd + lineEnd + "{1}" + lineEnd;
                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);

                    string formItem = "";

                    if (paramKeyValue.Length == 2)
                    {
                        formItem = string.Format(formdataTemplate, paramKeyValue[0], paramKeyValue[1]);
                    }
                    else
                    {
                        // if there are more/less than 2 values then set
                        // the key as blank
                        formItem = string.Format(formdataTemplate, paramKeyValue[0], "");
                    }

                    byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(formItem);
                    stream.Write(formItemBytes, 0, formItemBytes.Length);
                }
            }
        }

        /// <summary>
        /// Add file data to request stream
        /// </summary>
        /// <param name="stream">request stream</param>
        private void AddFileMetaInfo(Stream stream)
        {
            byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes(lineStart + boundary + lineEnd);

            foreach (FileInfoBean fileInfoItem in fileMetaInfoList)
            {
                if (fileInfoItem.FilePath == SmartConstants.FILE_DATA)
                {

                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; " + lineEnd + "Content-Type: {1}" + lineEnd + "Content-Transfer-Encoding: {2}" + lineEnd + lineEnd;
                    string header = string.Format(headerTemplate, fileInfoItem.FileName, this.MimeType, this.encoding);

                    byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(header);

                    stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                    stream.Write(headerBytes, 0, headerBytes.Length);

                    byte[] formItemBytes = Convert.FromBase64String(fileInfoItem.FileData);
                    stream.Write(formItemBytes, 0, formItemBytes.Length);

                }
                else if (fileInfoItem.FilePath == SmartConstants.FILE_URL)
                {
                    using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!isoFile.FileExists(fileInfoItem.FileData))
                        {
                            throw new MobiletException(ExceptionTypes.FILE_NOT_FOUND_EXCEPTION);
                        }

                        using (FileStream fileStream = new IsolatedStorageFileStream(fileInfoItem.FileData, FileMode.Open, isoFile))
                        {
                            string fileName = fileInfoItem.FileData.Split('/').ElementAt<string>(fileInfoItem.FileData.Split('/').Length - 1);
                            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" + lineEnd + "Content-Type: {2}" + lineEnd + "Content-Transfer-Encoding: {3}" + lineEnd + lineEnd;
                            string header = string.Format(headerTemplate, fileInfoItem.FileName, fileName, this.MimeType, this.encoding);
                            byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(header);

                            stream.Write(boundaryBytes, 0, boundaryBytes.Length);
                            stream.Write(headerBytes, 0, headerBytes.Length);
                            byte[] buffer = new byte[4096];
                            int bytesRead = 0;

                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                stream.Write(buffer, 0, bytesRead);

                            }
                            byte[] fileEnd = System.Text.Encoding.UTF8.GetBytes(lineEnd);
                            stream.Write(fileEnd, 0, fileEnd.Length);
                        }

                    }
                }
                else
                {
                    throw new MobiletException(ExceptionTypes.HTTP_PROCESSING_EXCEPTION);
                }
            }
        }

        /// <summary>
        /// Create File dump of response string in isolated storage.
        /// </summary>
        /// <param name="responseString">response string</param>
        private void CreateFileDump(Stream responseString)
        {
            responseString.Seek(0,SeekOrigin.Begin);
            using (IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication())
            {
                this.fileToSaveLocation =  requestFileToSaveName /*+ SmartConstants.FILE_TYPE_DAT*/;
                using (IsolatedStorageFileStream fileStream = userStoreForApplication.OpenFile(this.fileToSaveLocation, FileMode.Create))
                {
                    responseString.CopyTo(fileStream);
                }
                
            }
        }
        /// <summary>
        /// Check if url is of image type.
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckIfImageUrl()
        {
            bool isImageUrl = false;
            if (this.requestURL.EndsWith(".jpg") || this.requestURL.EndsWith(".jpeg") || this.requestURL.EndsWith(".png") || this.requestURL.EndsWith(".bmp") || this.requestURL.EndsWith(".gif"))
            {
                isImageUrl = true;
            }

            return isImageUrl;
        }
        /// <summary>
        /// Convert stream to byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                if (input.CanSeek)
                    input.Seek(0, SeekOrigin.Begin);
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}

using appez.constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Windows.Storage;

namespace appez.utility
{
    /// <summary>
    /// Utility for file operation.
    /// Support file read, folder read operation.
    /// </summary>
    public class FileReadUtility
    {
        #region variables
        private string fileInfo = null;
        private string fileContents = null;
        private JObject fileToReadInfo = null;
        private Dictionary<string, string> assetFileNameLocationMap = null;
        private string formatToRead = null;
        #endregion
        /// <summary>
        /// Creates the instance of FileReadUtility
        /// </summary>
        /// <param name="fileToRead">file name to be read.</param>
        public FileReadUtility(JObject fileToReadInformation)
        {
            assetFileNameLocationMap = new Dictionary<string, string>();
            if (fileToReadInformation != null)
            {
                this.fileToReadInfo = fileToReadInformation;
                this.fileInfo = fileToReadInformation.GetValue(CommMessageConstants.MMI_REQUEST_PROP_FILE_TO_READ_NAME).ToString();
            }
        }

        /// <summary>
        /// Read file content in assets folder
        /// </summary>
        /// <returns>File content in string</returns>
        public string GetFileContents()
        {
            string fileData = null;
            try
            {
                JObject fileContentsObj = new JObject();
                JArray fileContentsArray = new JArray();
                string filePath = "Assets/" + this.fileInfo;
                var resourcePath = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));
                fileContents = new StreamReader(resourcePath.Stream).ReadToEnd();

                // check if we are reading XML file
                if (fileInfo.EndsWith(SmartConstants.FILE_TYPE_XML))
                {

                    XmlNodeConverter converter = new XmlNodeConverter();
                    XDocument doc = XDocument.Parse(fileContents);
                    fileContents = JsonConvert.SerializeObject(doc, Formatting.None, converter);

                }

                // Replace all the single quotes with HTML encoded character equivalent
                fileContents = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileContents), 0, fileContents.Length);

                fileContents = fileContents.Replace("(\\r|\\n|\\t)", "").Replace("\\r\\n", "");
                // construct the JSON object which contains the file contents

                JObject fileContent = new JObject();
                fileContent.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_NAME, fileInfo);
                fileContent.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_CONTENT, fileContents);
                fileContent.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_TYPE, "");
                fileContent.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_SIZE, 0);
                fileContentsArray.Add(fileContent);

                fileContentsObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_CONTENTS, fileContentsArray);

                fileData = fileContentsObj.ToString();
            }
            catch (JsonException)
            {
                fileData = null;
            }
            return fileData;

        }


        /// <summary>
        /// Provides the contents of all the files of a particular type in the
        /// specified folder. It does not read files in subfolders
        /// </summary>
        /// <returns>Well formatted JSON response containing the
        /// contents of the specified types of file in the folder</returns>
        public async Task<string> GetFileContentInFolder()
        {
            String filesData = null;
            bool isFileExist = false;

            JObject folderReadDetails = this.fileToReadInfo;
            JToken tempToken = null;
            if (folderReadDetails.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_FILE_TO_READ_NAME, out tempToken))
            {
                String folderNameToRead = folderReadDetails.GetValue(CommMessageConstants.MMI_REQUEST_PROP_FILE_TO_READ_NAME).ToString();
                formatToRead = folderReadDetails.GetValue(CommMessageConstants.MMI_REQUEST_PROP_FOLDER_FILE_READ_FORMAT).ToString();
                if (folderReadDetails.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_FOLDER_READ_SUBFOLDER, out tempToken) && Convert.ToBoolean(folderReadDetails.GetValue(CommMessageConstants.MMI_REQUEST_PROP_FOLDER_READ_SUBFOLDER).ToString()))
                {
                    // That means the user has specified to read all the files
                    // of the specified format in the subfolders also.
                    isFileExist = await ListAssetFilesFullDepth("Assets/" + folderNameToRead);
                }
                else
                {
                    // This means user has specified to read files of provided
                    // format in the current folder only
                    isFileExist = await ListAssetFilesInSpecifiedFolder("Assets/" + folderNameToRead);
                }

                JObject fileContentList = new JObject();
                JArray filesArray = new JArray();

                if (isFileExist && assetFileNameLocationMap != null && assetFileNameLocationMap.Count > 0)
                {

                    foreach (var pairs in assetFileNameLocationMap)
                    {
                        JObject fileNode = new JObject();
                        var resourcePath = Application.GetResourceStream(new Uri(pairs.Value, UriKind.Relative));
                        fileContents = new StreamReader(resourcePath.Stream).ReadToEnd();
                        fileContents = Convert.ToBase64String(Encoding.UTF8.GetBytes(fileContents), 0, fileContents.Length);
                        fileContents = fileContents.Replace("(\\r|\\n|\\t)", "").Replace("\\r\\n", "");

                        fileNode.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_NAME, pairs.Key);
                        fileNode.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_CONTENT, fileContents);
                        fileNode.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_TYPE, "");
                        fileNode.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_SIZE, 0);

                        filesArray.Add(fileNode);
                    }

                    fileContentList.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_CONTENTS, filesArray);
                    filesData = fileContentList.ToString();
                }
            }
            return filesData;
        }

        /// <summary>
        /// This method returns the list of all the files and their path w.r.t. 'Assets' folder ONLY IN the specified folder
        /// </summary>
        /// <param name="folderNameToRead">Path in which files need to be searched and read</param>
        /// <returns></returns>
        private async Task<bool> ListAssetFilesInSpecifiedFolder(string folderNameToRead)
        {
            try
            {
                StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(folderNameToRead);
                IReadOnlyList<IStorageItem> folderitems = await sf.GetItemsAsync();
                foreach (var item in folderitems)
                {
                    if (item.Name.EndsWith(formatToRead))
                    {
                        assetFileNameLocationMap.Add(item.Name, item.Path);
                    }
                }

            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This method returns the list of all the files and their path w.r.t. 'Assets' folder EVEN IN SUBFOLDERS of the specified folder
        /// </summary>
        /// <param name="folderNameToRead">Path in which files need to be searched and read</param>
        /// <returns></returns>
        private async Task<bool> ListAssetFilesFullDepth(string folderNameToRead)
        {

            try
            {

                StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(folderNameToRead);
                IReadOnlyList<IStorageItem> folderitems = await sf.GetItemsAsync();
                foreach (var item in folderitems)
                {
                    if (item.Name.EndsWith(formatToRead))
                    {
                        assetFileNameLocationMap.Add(item.Name, item.Path);
                    }
                    if (item.IsOfType(StorageItemTypes.Folder))
                    {
                        await ListAssetFilesFullDepth(item.Path);
                    }
                }

            }
            catch (IOException)
            {
                return false;
            }
            return true;
        }
    }
}

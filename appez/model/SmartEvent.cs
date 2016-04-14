using appez.constants;
using appez.exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Xml.Linq;


namespace appez.model
{
    /// <summary>
    /// Holds the necessary parameters for Smart object. Parses the smart message
    /// received from JavaScript to extract parameter values
    /// </summary>
    public class SmartEvent
    {

       
        
        public const string JS_CALLBACK_FUNCTION = "appez.mmi.getMobiletManager().processNativeResponse";

	    // Communication message properties
        // Unique ID associated with each request initiated by the web layer
        public string TransactionId { get; set; }
        // Indicates whether or not the web layer is waiting for a response for this
        // operation
        public bool IsResponseExpected { get; set; }
        // Checks the validity of the message received from the web layer
        public bool IsValidProtocol { get; set; }
        // Request data associated with request for a particular service. This
        // request will be in the form of JSON
        public string ServiceRequestData { get; set; }
        public int EventType { set; get; }
        public int ServiceType { set; get; }
        // Holds the instance of SmartEventRequest which is constructed from the
        // user request received from the web layer
        public SmartEventRequest SmartEventRequest
        {
            set
            {
                if (value != null)
                    smartEventRequest = value;
            }

            get
            {
                return smartEventRequest;
            }
        }
        // Holds the SmartEventResponse instance that needs to be sent to web layer
        public SmartEventResponse SmartEventResponse
        {
            set
            {
                if(value!=null)
                    smartEventResponse = value;
            }

            get
            {
                return smartEventResponse;
            }
        }
	    
	    private SmartEventRequest smartEventRequest = null;
        private SmartEventResponse smartEventResponse = null;
        // Response string that is collection of service request and response data
        // that needs to be sent back to the web layer
	    private string jsNameToCallArg = null;

        /// <summary>
        /// Parses the SmartEvent protocol message received from the web layer to
        /// create a SmartEvent object
        /// </summary>
        /// <param name="message">Smart event encoded string.</param>
        public SmartEvent(string message)
        {
            if (null == message)
            {
                throw new MobiletException(ExceptionTypes.INVALID_PROTOCOL_EXCEPTION);
            }
            try
            {
                if (message != null && message.Length > 0)
                {
                    JObject smartEventObj = JObject.Parse(message);

                    this.TransactionId = smartEventObj.GetValue(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_ID).ToString();
                    this.IsResponseExpected = (bool)smartEventObj.GetValue(CommMessageConstants.MMI_MESSAGE_PROP_RESPONSE_EXPECTED);

                    smartEventRequest = new SmartEventRequest();
                    int serviceOperationId = (int)((JObject)smartEventObj.GetValue(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_REQUEST)).GetValue(CommMessageConstants.MMI_MESSAGE_PROP_REQUEST_OPERATION_ID);
                    EventType = ParseEventType(serviceOperationId);
                    ServiceType = ParseServiceType(serviceOperationId);
                    smartEventRequest.ServiceOperationId = serviceOperationId;

                    string serviceRequestData = ((JObject)smartEventObj.GetValue(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_REQUEST)).GetValue(CommMessageConstants.MMI_MESSAGE_PROP_REQUEST_DATA).ToString();
                    byte[] serviceRequestDataArray = Convert.FromBase64String(serviceRequestData);
                    serviceRequestData = UTF8Encoding.UTF8.GetString(serviceRequestDataArray,0,serviceRequestDataArray.Length);
                    JObject serviceReqObj = JObject.Parse(serviceRequestData);
                    smartEventRequest.ServiceRequestData = serviceReqObj;
                    JToken tempToken;
                    if (serviceReqObj.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_SERVICE_SHUTDOWN,out tempToken))
                    {
                        smartEventRequest.ServiceShutdown=Convert.ToBoolean(tempToken.ToString());
                    }
                    else
                    {
                        smartEventRequest.ServiceShutdown=true;
                    }

                    smartEventResponse = new SmartEventResponse();

                    IsValidProtocol = true;
                }
            }
            catch (ArgumentException)
            {
                IsValidProtocol = false;
            }
            catch (JsonException)
            {
                IsValidProtocol = false;
            }
            catch (InvalidCastException)
            {
                IsValidProtocol = false;
            }
            catch (Exception)
            {
                IsValidProtocol = false;
            }
        }

        /// <summary>
        /// This method does the parsing of event type from given 5 digit number
        /// [0][1][2][3][4]. and we are extracting int digit available at 0'th place.
        /// </summary>
        /// <param name="actionCode">actionCode</param>
        /// <returns>int</returns>
        private int ParseEventType(int actionCode)
        {
            return actionCode / 10000;
        }

        /// <summary>
        /// This method does the parsing of service type from given 5 digit number
        /// [0][1][2][3][4], & we are extracting int digits available at 1'st and
        /// 2'nd place.
        /// </summary>
        /// <param name="actionCode">actionCode</param>
        /// <returns>int</returns>
        private int ParseServiceType(int actionCode)
        {
            int tmpNum = actionCode % 10000;
            return tmpNum / 100;
        }
        public String GetJavaScriptNameToCall()
        {
            return JS_CALLBACK_FUNCTION;
        }

        public void SetJavaScriptNameToCallArg(String argument)
        {
            this.jsNameToCallArg = argument;
        }
        /// <summary>
        /// Returns the complete and well formed response to be sent to the web layer
        /// for user initiated request
        /// </summary>
        /// <returns>Well formed response for the web layer</returns>
        public string GetJavaScriptNameToCallArg()
        {

           
            try
            {
                JObject callbackResponseObj = new JObject();
                callbackResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_ID, this.TransactionId);
                callbackResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_RESPONSE_EXPECTED, this.IsResponseExpected);

                JObject transactionRequestObj = new JObject();
                transactionRequestObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_REQUEST_OPERATION_ID, this.GetServiceOperationId());
                String serviceRequestData = this.SmartEventRequest.ServiceRequestData.ToString();
                serviceRequestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(serviceRequestData), 0, serviceRequestData.Length);
                serviceRequestData = serviceRequestData.Replace("\\n", "");
                transactionRequestObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_REQUEST_DATA, serviceRequestData);
                callbackResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_REQUEST, transactionRequestObj);

                JObject transactionResponseObj = new JObject();
                transactionResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_OP_COMPLETE, this.SmartEventResponse.IsOperationComplete);
                string serviceResponseData = this.SmartEventResponse.ServiceResponse;
                serviceResponseData = Convert.ToBase64String(Encoding.UTF8.GetBytes(serviceResponseData), 0, serviceResponseData.Length);
                serviceResponseData = serviceResponseData.Replace("\\n", "");
                transactionResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_SERVICE_RESPONSE, serviceResponseData);
                transactionResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_RESPONSE_EX_TYPE, this.SmartEventResponse.ExceptionType);
                transactionResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_RESPONSE_EX_MESSAGE, this.SmartEventResponse.ExceptionMessage);
                callbackResponseObj.Add(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_RESPONSE, transactionResponseObj);

                this.jsNameToCallArg = JsonConvert.SerializeObject(callbackResponseObj,Formatting.None);
            }
            catch (JsonException)
            {
                this.jsNameToCallArg = null;
            }
            catch (Exception)
            {
                this.jsNameToCallArg = null;
            }
            return this.jsNameToCallArg;
        }

        public int GetServiceOperationId()
        {
            return this.SmartEventRequest.ServiceOperationId;
        }
    }
}

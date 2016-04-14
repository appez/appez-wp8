using appez.constants;
using appez.exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.model
{
    /// <summary>
    /// Holds the necessary parameters for notifier object. Parses the notifier message
    /// received from JavaScript to extract parameter values
    /// </summary>
    public class NotifierEvent
    {
        public string TransactionId { get; set; }
        public int Type { get; set; }
        public int ActionType { get; set; }
        public bool IsOperationSuccess { get; set; }
        public int ErrorType { get; set; }

        private JObject notifierRequestData;

        public JObject RequestData
        {
            get
            {
                if (notifierRequestData == null)
                {
                    notifierRequestData = new JObject();
                } return notifierRequestData;
            }
            set { notifierRequestData = value; }
        }
        private JObject eventResponse;
        public JObject EventResponse
        {
            get
            {
                if (eventResponse == null)
                {
                    eventResponse = new JObject();
                } return eventResponse;
            }
            set { eventResponse = value; }
        }
        private string errorMessage;
        public string ErrorMessage
        {
            get
            {
                if (errorMessage == null)
                {
                    errorMessage = "";
                }
                return errorMessage;
            }
            set { errorMessage = value; }
        }

        public NotifierEvent()
        { 
        
        }

        public NotifierEvent(String notifierMessage)
        {
            try
            {
                JObject notifierRequest = JObject.Parse(notifierMessage);
                notifierRequest = JObject.Parse(notifierRequest.GetValue(NotifierMessageConstants.NOTIFIER_PROP_TRANSACTION_REQUEST).ToString());
                string notifierReqData = notifierRequest.GetValue(NotifierMessageConstants.NOTIFIER_REQUEST_DATA).ToString();
                byte[] notifierRequestDataArray = Convert.FromBase64String(notifierReqData);
                notifierReqData = UTF8Encoding.UTF8.GetString(notifierRequestDataArray, 0, notifierRequestDataArray.Length);
                this.Type = Convert.ToInt32(notifierRequest.GetValue(NotifierMessageConstants.NOTIFIER_TYPE).ToString());
                this.ActionType = Convert.ToInt32(notifierRequest.GetValue(NotifierMessageConstants.NOTIFIER_ACTION_TYPE).ToString());
                this.RequestData = JObject.Parse(notifierReqData);
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }

        /// <summary>
        /// Prepares the notifier response to be sent to the web layer
        /// </summary>
        /// <returns>string</returns>
        public String GetJavaScriptNameToCallArg()
        {
            String jsNameToCallArg = null;
            try
            {
                JObject jsNameToCallArgObj = new JObject();
                jsNameToCallArgObj.Add(NotifierMessageConstants.NOTIFIER_PROP_TRANSACTION_ID, this.TransactionId);

                JObject transactionRequestObj = new JObject();
                transactionRequestObj.Add(NotifierMessageConstants.NOTIFIER_TYPE, this.Type);
                transactionRequestObj.Add(NotifierMessageConstants.NOTIFIER_ACTION_TYPE, this.ActionType);
                String notifierRequestData = this.RequestData.ToString();
                notifierRequestData = Convert.ToBase64String(Encoding.UTF8.GetBytes(notifierRequestData), 0, notifierRequestData.Length);
                notifierRequestData = notifierRequestData.Replace("\\n", "");
                transactionRequestObj.Add(NotifierMessageConstants.NOTIFIER_REQUEST_DATA, notifierRequestData);
                jsNameToCallArgObj.Add(NotifierMessageConstants.NOTIFIER_PROP_TRANSACTION_REQUEST, transactionRequestObj);

                JObject transactionResponseObj = new JObject();
                transactionResponseObj.Add(NotifierMessageConstants.NOTIFIER_OPERATION_IS_SUCCESS, this.IsOperationSuccess);
                transactionResponseObj.Add(NotifierMessageConstants.NOTIFIER_OPERATION_ERROR_TYPE, this.ErrorType);
                transactionResponseObj.Add(NotifierMessageConstants.NOTIFIER_OPERATION_ERROR, this.ErrorMessage);
                String notifierResponseData = this.EventResponse.ToString();
                notifierResponseData = Convert.ToBase64String(Encoding.UTF8.GetBytes(notifierResponseData),0,notifierResponseData.Length);
                notifierResponseData = notifierResponseData.Replace("\\n", "");
                transactionResponseObj.Add(NotifierMessageConstants.NOTIFIER_EVENT_RESPONSE, notifierResponseData);
                jsNameToCallArgObj.Add(NotifierMessageConstants.NOTIFIER_PROP_TRANSACTION_RESPONSE, transactionResponseObj);

                jsNameToCallArg = jsNameToCallArgObj.ToString();
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            return jsNameToCallArg;
        }

    }
}

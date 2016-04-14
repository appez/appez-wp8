using appez.constants;
using appez.listeners;
using appez.model;
using appez.utility;
using System;

namespace appez.services
{
    /// <summary>
    /// Performs HTTP operations. Currently supports HTTP GET and POST
    /// operations. Also supports feature to create a DAT file dump that holds the response of HTTP operation
    /// </summary>
    public class HttpService : SmartService, SmartNetworkListener
    {
        #region variables
        public static HttpService httpService = null;
        private SmartEvent smartEvent = null;
        private SmartServiceListener smartServiceListener = null;
        #endregion
        /// <summary>
        /// Creates the instance of HttpService
        /// </summary>
        /// <param name="smartServiceListener">SmartServiceListener</param>
        public HttpService(SmartServiceListener smartServiceListener)
            : base()
        {

            this.smartServiceListener = smartServiceListener;
        }


        public override void ShutDown()
        {
            smartServiceListener = null;
        }

        /// <summary>
        /// Performs HTTP action based on SmartEvent action type
        /// </summary>
        /// <param name="smartEvent">SmartEvent specifying action type for the HTTP action</param>
        public override void PerformAction(SmartEvent smartEvent)
        {
            this.smartEvent = smartEvent;

            HttpUtility service = new HttpUtility(this);
            bool createFile = this.smartEvent.GetServiceOperationId() == WebEvents.WEB_HTTP_REQUEST_SAVE_DATA ? true : false;
            service.PerformAsyncRequest(this.smartEvent.SmartEventRequest.ServiceRequestData.ToString(), createFile);
        }

        /// <summary>
        /// Updates SmartEventResponse and thereby SmartEvent based on the HTTP
        /// operation performed in NetworkService. Also notifies SmartServiceListener
        /// about successful completion of HTTP operation
        /// </summary>
        /// <param name="responseData">HTTP response data</param>
        public void OnSuccessHttpOperation(string responseData)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete=true;
            smEventResponse.ServiceResponse=responseData;
            smEventResponse.ExceptionType=0;
            smEventResponse.ExceptionMessage=null;
            smartEvent.SmartEventResponse=smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }

        /// <summary>
        /// Notifies SmartServiceListener about unsuccessful completion of HTTP
        /// operation
        /// </summary>
        /// <param name="exceptionType">Exception type</param>
        /// <param name="exceptionMessage">Message describing the type of exception</param>
        public void OnErrorHttpOperation(int exceptionType, String exceptionMessage)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete=false;
            // TODO set the response string here
            smEventResponse.ServiceResponse=null;
            smEventResponse.ExceptionType=exceptionType;
            smEventResponse.ExceptionMessage=exceptionMessage;
            smartEvent.SmartEventResponse=smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(smartEvent);
        }

    }
}

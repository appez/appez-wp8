using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.services;
using appez.utility;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Diagnostics;
using System.Text;


namespace appez
{
    /// <summary>
    /// Responsible for rendering of WEB-EVENTs. Uses
    /// services of SmartServiceRouter to get allocation of desired service as per
    /// the demand of WEB-EVENTs. In case of APP-EVENT it just passes notification
    /// back to SmartConnector using SmartEventListener.It implements
    /// SmartServiceListener to get processing updates of active SmartService.
    /// </summary>
    public class SmartEventProcessor : SmartServiceListener
    {

        private SmartEventListener smartEventListener = null;
        private SmartServiceRouter smartServiceRouter = null;
        private bool isBusy = false;

        public SmartEventProcessor(SmartEventListener smartEventListener)
        {
            this.smartEventListener = smartEventListener;
            if (this.smartServiceRouter == null)
            {
                this.smartServiceRouter = new SmartServiceRouter(this);
            }

        }

        public void ShutDown()
        {
            smartEventListener = null;
            smartServiceRouter = null;
        }

        public void SetSmartServiceRouter(SmartServiceRouter smServiceRouter)
        {
            this.smartServiceRouter = smServiceRouter;
        }


        /// <summary>
        /// Processes SmartEvent received from SmartConnector. In case of WEB-EVENT
        /// it delegates responsibility to SmartServiceRouter. In case of APP-EVENT
        /// it send back notification to SmartConnector
        /// </summary>
        /// <param name="smartEvent">SmartEvent to be processed</param>
        public void ProcessSmartEvent(SmartEvent smartEvent)
        {
            int eventType = smartEvent.EventType;
            switch (eventType)
            {
                case SmartConstants.WEB_EVENT:
                    HandleWebEvent(smartEvent);
                    break;

                case SmartConstants.CO_EVENT:
                    HandleCoEvent(smartEvent);
                    break;

                case SmartConstants.APP_EVENT:
                    HandleAppEvent(smartEvent);
                    break;
            }
            
        }


        /// <summary>
        /// Specifies action to be taken on the basis of type SmartEvent service.
        /// Initialises SmartService via SmartServiceRouter and performs desired
        /// Action
        /// </summary>
        /// <param name="smartEvent">SmartEvent that specifies service to be processed</param>
        private void HandleWebEvent(SmartEvent smartEvent)
        {
            int serviceType = smartEvent.ServiceType;
            SmartService smartService = null;
            if (!isBusy)
            {
                isBusy = true;
                smartService = smartServiceRouter.GetService(serviceType);

                if (smartService != null)
                {
                    smartService.PerformAction(smartEvent);
                }

            }
            else
            {
                Debug.WriteLine("**********ANOTHER SMARTEVENT UNDER PROCESS**********");
            }
        }

        private void HandleCoEvent(SmartEvent smartEvent)
        {
            int serviceType = smartEvent.ServiceType;
            bool isValidServiceType = true;
            SmartService smartService = null;
            switch (serviceType)
            {
                case ServiceConstants.CONTEXT_CHANGE_SERVICE:
                    // Done for handling CONTEXT events such as
                    // CONTEXT_WEBVIEW_SHOW, CONTEXT_WEBVIEW_HIDE and
                    // CONTEXT_NOTIFICATION_CREATELIST(and others) since they require the
                    // control to be transferred to the implementation of
                    // SmartConnectorListener(in this case SmartViewActivity)

                    //TODO discuss the validity of this approach. Since this is used for the time being till context service is not introduced
                    smartEventListener.OnCompleteActionWithSuccess(smartEvent);
                    break;

                case ServiceConstants.MAPS_SERVICE:
                    if (!SessionData.GetInstance().GetIsMapBusy())
                    {
                        SessionData.GetInstance().SetIsMapBusy(true);
                        smartService = smartServiceRouter.GetService(ServiceConstants.MAPS_SERVICE);
                    }
                    break;

                default:
                    isValidServiceType = false;
                    break;
            }

            if (!isValidServiceType)
            {
                OnCompleteServiceWithError(smartEvent);
            }
            if (smartService != null)
            {
                smartService.PerformAction(smartEvent);
            }
            
        }

        private void HandleAppEvent(SmartEvent smartEvent)
        {
            smartEventListener.OnCompleteActionWithSuccess(smartEvent);
        }


        /// <summary>
        /// Sends completion notification of success to intended client using
        /// SmartEventListener
        /// </summary>
        /// <param name="smartEvent">SmartEvent on which success notification is received from
        ///  SmartService</param>
        public void OnCompleteServiceWithSuccess(SmartEvent smartEvent)
        {
            isBusy = false;
            if (smartServiceRouter != null)
            {
                // While releasing service instance, check whether the service has
                // specified to shutdown itself and also whether or not the
                // operation has
                smartServiceRouter.ReleaseService(smartEvent.ServiceType, smartEvent.SmartEventRequest.ServiceShutdown);
            }
            smartEventListener.OnCompleteActionWithSuccess(smartEvent);
        }

        /// <summary>
        /// Sends error notification to intended client using SmartEventListener
        /// </summary>
        /// <param name="smartEvent">SmartEvent on which error notification is received from
        /// SmartService</param>
        public void OnCompleteServiceWithError(SmartEvent smartEvent)
        {
            isBusy = false;
            if (smartServiceRouter != null)
            {
                smartServiceRouter.ReleaseService(smartEvent.ServiceType, smartEvent.SmartEventRequest.ServiceShutdown);
            }
            smartEventListener.OnCompleteActionWithError(smartEvent);
        }
        
    }
}

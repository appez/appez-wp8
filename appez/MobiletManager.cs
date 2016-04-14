using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using Newtonsoft.Json;
using System;


namespace appez
{
    /// <summary>
    /// Responsible for checking sanity of event using
    /// smartEvent. Also provides valid smart event to SmartEventProcessor for
    /// further processing. It implements SmartEventLister to get processing results
    /// from SmartEventProcessor. Events could be of two types name as WEB-EVENT and
    /// APP-EVENT. Smart Connector exposes sends notification of WEB-EVENT processing
    /// to SmartViewActivity using SmartConnnectorListener and send APP-EVENT using
    /// SmartAppListener
    /// </summary>
    public class MobiletManager : SmartEventListener
    {
        #region variables
        private SmartConnectorListener smartConnectorListener = null;
        private SmartAppListener smartAppListener = null;
        private SmartEventProcessor smartEventProcessor = null;
        #endregion

        public MobiletManager()
        {
        }

        public MobiletManager(SmartConnectorListener argMobiletManagerListener, SmartAppListener argSmartAppListener)
        {

            if (argMobiletManagerListener == null)
            {
                throw new MobiletException(ExceptionTypes.SMART_CONNECTOR_LISTENER_NOT_FOUND_EXCEPTION);
            }

            if (argSmartAppListener == null)
            {
                throw new MobiletException(ExceptionTypes.SMART_APP_LISTENER_NOT_FOUND_EXCEPTION);
            }

            this.smartConnectorListener = argMobiletManagerListener;
            this.smartAppListener = argSmartAppListener;
        }

        public MobiletManager(SmartConnectorListener argMobiletManagerListener)
        {

            if (argMobiletManagerListener == null)
            {
                throw new MobiletException(ExceptionTypes.SMART_CONNECTOR_LISTENER_NOT_FOUND_EXCEPTION);
            }

            this.smartConnectorListener = argMobiletManagerListener;

        }


        /// <summary>
        /// Sets target to receive SmartAppListener Notifications
        /// </summary>
        /// <param name="argSmartAppListener">Reference of outer activity</param>
        public void RegisterAppListener(SmartAppListener argSmartAppListener)
        {
            if (argSmartAppListener == null)
            {
                throw new MobiletException(ExceptionTypes.SMART_APP_LISTENER_NOT_FOUND_EXCEPTION);
            }

            this.smartAppListener = argSmartAppListener;
        }



        public void ShutDown()
        {
            if (smartEventProcessor != null)
            {
                smartEventProcessor.ShutDown();
            }
            smartConnectorListener = null;
            smartEventProcessor = null;
            smartAppListener = null;
        }


        /// <summary>
        /// Sets the value of SmartEventProcessor
        /// </summary>
        /// <param name="smEventProcessor"></param>
        public void SetSmartEventProcessor(SmartEventProcessor smEventProcessor)
        {
            this.smartEventProcessor = smEventProcessor;
        }


        /// <summary>
        /// Processes SmartEvent after validating the sanity of event.
        /// </summary>
        /// <param name="message">Message to be processed</param>
        /// <returns>bool</returns>
        public bool ProcessSmartEvent(string message)
        {
            bool isValidEvent = false;
            SmartEvent smartEvent = new SmartEvent(message);
            isValidEvent = ProcessSmartEvent(smartEvent);

            return isValidEvent;
        }

        /// <summary>
        /// Overloaded version of processSmartEvent, requires SmartEvent as a input.
        /// </summary>
        /// <param name="smartEvent">Message to be processed</param>
        /// <returns>bool</returns>
        public bool ProcessSmartEvent(SmartEvent smartEvent)
        {
            bool isValidEvent = false;
            isValidEvent = smartEvent.IsValidProtocol;

            if (isValidEvent)
            {
                if (smartEventProcessor == null)
                {
                    smartEventProcessor = new SmartEventProcessor(this);
                }
                this.smartEventProcessor.ProcessSmartEvent(smartEvent);
            }

            return isValidEvent;
        }


        /// <summary>
        /// Sends failure notification to intended client in event of erroneous
        /// completion of smart service action.
        /// </summary>
        /// <param name="smartEvent">SmartEvent received after erroneous completion
        /// of smartservice action</param>
        public void OnCompleteActionWithError(SmartEvent smartEvent)
        {
            this.smartConnectorListener.OnFinishProcessingWithError(smartEvent);
        }


        /// <summary>
        /// Sends success notification to intended client in event of successful
        /// completion of action. Also determines the type of notification and
        /// accordingly sends notification to either App Listener or Connector
        /// Listener or both.
        /// </summary>
        /// <param name="smartEvent">SmartEvent received after successful completion of smart
        /// service action</param>
        public void OnCompleteActionWithSuccess(SmartEvent smartEvent)
        {
            int eventType = smartEvent.EventType;
            string notification = "" + smartEvent.GetServiceOperationId();
            string eventData = smartEvent.ServiceRequestData;
            
            switch (eventType)
            {
                case SmartConstants.WEB_EVENT:
                    // TODO : Need to discuss there should be one notification at a time
                    this.smartConnectorListener.OnFinishProcessingWithOptions(smartEvent);
                    
                    break;

                case SmartConstants.CO_EVENT:

                    this.smartConnectorListener.OnReceiveContextNotification(smartEvent);
                    break;

                case SmartConstants.APP_EVENT:
                    // Check the value of notification, notify to App listener in case
                    // of valid data
                    try {
				        eventData = smartEvent.SmartEventRequest.ServiceRequestData.GetValue(CommMessageConstants.MMI_REQUEST_PROP_MESSAGE).ToString();
				    
                        if (notification != null)
                        {
                            this.smartAppListener.OnReceiveSmartNotification(eventData, notification);
                        }
                        
			        } catch(JsonException){
				        this.smartAppListener.OnReceiveSmartNotification(null, null);
			        }
                    break;
            }
        }


    }
}

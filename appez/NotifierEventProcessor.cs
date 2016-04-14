using appez.constants;
using appez.listeners;
using appez.listeners.notifier;
using appez.model;
using appez.notifier;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace appez
{
    /// <summary>
    /// Responsible for processing of NOTIFIER-EVENTs. Uses
    /// services of NotifierEventRouter to get allocation of desired notifier as per
    /// the demand of NOTIFIER-EVENTs.
    /// </summary>
    public class NotifierEventProcessor : NotifierEventListener
    {

        private SmartNotifierListener smartNotifierListener = null;

        public NotifierEventProcessor(SmartNotifierListener smNotifierListener)
        {
            this.smartNotifierListener = smNotifierListener;
        }
        /// <summary>
        /// Processes NotifierEvent received. It delegates responsibility to NotifierEventRouter. 
        /// It register notifier event for further notification when event occurs.
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void ProcessNotifierRegistrationReq(NotifierEvent notifierEvent)
        {
            NotifierEventRouter notifierEventRouter = new NotifierEventRouter(this);

            int notifierType = 0;
            int notifierActionType = 0;
            
            if (notifierEvent != null)
            {
                notifierType = notifierEvent.Type;
                notifierActionType = notifierEvent.ActionType;
            }

            SmartNotifier smartNotifier = notifierEventRouter.GetNotifier(notifierType);

            if (smartNotifier != null)
            {
                if (notifierActionType == NotifierConstants.NOTIFIER_ACTION_REGISTER)
                {
                    smartNotifier.RegisterListener(notifierEvent);
                }
                else if (notifierActionType == NotifierConstants.NOTIFIER_ACTION_UNREGISTER)
                {
                    smartNotifier.UnregisterListener(notifierEvent);
                }
                else
                {
                    // Do nothing.
                }
            }
            
        }
        /// <summary>
        /// Sends completion notification of success to intended client using
	    /// NotifierEventListener
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void OnNotifierEventReceivedSuccess(NotifierEvent notifierEvent)
        {
            this.smartNotifierListener.OnReceiveNotifierEventSuccess(notifierEvent);
        }

        /// <summary>
        /// Sends completion notification of successful registration to intended client using
        /// NotifierEventListener
        /// </summary>
        /// <param name="notifierEvent"></param>
	
	    public void OnNotifierRegistrationCompleteSuccess(NotifierEvent notifierEvent) {
		    this.smartNotifierListener.OnReceiveNotifierRegistrationEventSuccess(notifierEvent);
	    }

	    /// <summary>
        /// Sends error notification to intended client using NotifierEventListener
	    /// </summary>
	    /// <param name="notifierEvent"></param>
	    public void OnNotifierEventReceivedError(NotifierEvent notifierEvent) {
		    this.smartNotifierListener.OnReceiveNotifierEventError(notifierEvent);
	    }
        /// <summary>
        /// Sends error notification of registration to intended client using NotifierEventListener
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void OnNotifierRegistrationCompleteError(NotifierEvent notifierEvent)
        {
            this.smartNotifierListener.OnReceiveNotifierRegistrationEventError(notifierEvent);
        }
    }
}

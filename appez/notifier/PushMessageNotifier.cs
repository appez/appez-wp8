using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.listeners.notifier;
using appez.model;
using appez.utility;
using appez.utility.push;
using Microsoft.Phone.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace appez.notifier
{
    /// <summary>
    /// Responsible for notifying the registered
    /// applications whenever a push notification is received from the server
    /// </summary>
    public class PushMessageNotifier : SmartNotifier, NotifierPushMessageListener
    {
        
        private NotifierEventListener notifierEventListener = null;
        
        public PushMessageNotifier(NotifierEventListener notifierEvListener)
        {
            this.notifierEventListener = notifierEvListener;
        }

        public override void RegisterListener(NotifierEvent notifierEvent)
        {
            PushNotificationUtility pushNotificationUtility = new PushNotificationUtility(this);
            pushNotificationUtility.Register(notifierEvent);

        }


        public override void UnregisterListener(NotifierEvent notifierEvent)
        {
            PushNotificationUtility pushNotificationUtility = new PushNotificationUtility(this);
            pushNotificationUtility.Unregister(notifierEvent);
        }

       
        public void OnPushEventReceivedSuccess(NotifierEvent notifierEvent)
        {
            notifierEventListener.OnNotifierEventReceivedSuccess(notifierEvent);
        }

        public void OnPushEventReceivedError(NotifierEvent notifierEvent)
        {
            notifierEventListener.OnNotifierEventReceivedError(notifierEvent);
        }

        public void OnPushRegistrationCompleteSuccess(NotifierEvent notifierEvent)
        {
            notifierEventListener.OnNotifierRegistrationCompleteSuccess(notifierEvent);
        }

        public void OnPushRegistrationCompleteError(NotifierEvent notifierEvent)
        {
            notifierEventListener.OnNotifierRegistrationCompleteError(notifierEvent);
        }
    }
}

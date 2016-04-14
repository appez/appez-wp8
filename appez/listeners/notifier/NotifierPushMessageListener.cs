using appez.model;

namespace appez.listeners.notifier
{
    /// <summary>
    /// Defines an interface to listen push message events.
    /// </summary>
    public interface NotifierPushMessageListener
    {
        /// <summary>
        /// This method is called on successful receiving of push message.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnPushEventReceivedSuccess(NotifierEvent notifierEvent);

        /// <summary>
        /// This method is called if there's an error in receiving a push message.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnPushEventReceivedError(NotifierEvent notifierEvent);

        /// <summary>
        /// This method is called on successful registration of device for push notification.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnPushRegistrationCompleteSuccess(NotifierEvent notifierEvent);

        /// <summary>
        /// This method is called if there's error registering device with UPNS server.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnPushRegistrationCompleteError(NotifierEvent notifierEvent);
    }
}

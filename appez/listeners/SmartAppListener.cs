using System;

namespace appez.listeners
{
    /// <summary>
    /// Defines an Interface for listening to JavaScript
    /// notifications meant for the application. User application
    /// classes('AppViewModel') also listens for these events and hence implement
    /// this interface
    /// </summary>
    public interface SmartAppListener
    {
      
        /// <summary>
        /// Specifies action to be taken on receiving Smart notification containing
	    /// event data and notification.
        /// </summary>
        /// <param name="eventData">Event data</param>
        /// <param name="notification">Notification from Javascript</param>
        void OnReceiveSmartNotification(string eventData, string notification);

        /// <summary>
        /// Specifies action to be taken on receiving Smart notification containing
	    /// event data, notification and data message.
        /// </summary>
        /// <param name="notification">Notification from Javascript</param>
        /// <param name="fromFile">Data message containing file name that holds response from
	    /// called action</param>
        void OnReceiveDataNotification(string notification, string fromFile);

        /// <summary>
        /// Specifies action to be taken on receiving Smart notification containing
	    /// event data and notification.
        /// </summary>
        /// <param name="notification">Notification from Javascript</param>
        /// <param name="responseData">Response of called action</param>
        void OnReceiveDataNotification(string notification, byte[] responseData);

    }
}

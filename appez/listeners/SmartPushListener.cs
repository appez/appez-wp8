
namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for listening push notification events.
    /// This include listening to push message recieve, error on push message.
    /// </summary>
    public interface SmartPushListener
    {
        /// <summary>
        /// Specifies action to be taken on push message recieved
        /// </summary>
        /// <param name="pushNotification"></param>
        void OnMessage(string pushNotification);
        /// <summary>
        /// Specifies action to be taken on push message delete.
        /// </summary>
        /// <param name="total"></param>
        void OnDeletedMessages(int total);
        /// <summary>
        /// Specified action to be taken on push message error.
        /// </summary>
        /// <param name="errorId"></param>
        void OnError(string errorId);
        /// <summary>
        /// Specified action to be taken on recoverable error.
        /// </summary>
        /// <param name="errorId"></param>
        void OnRecoverableError(string errorId);
    }
}

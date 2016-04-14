
using appez.model;
using Newtonsoft.Json.Linq;
namespace appez.listeners.notifier
{
    /// <summary>
    /// Defines an interface to listen notifier events.
    /// </summary>
    public interface NotifierEventListener
    {
        /// <summary>
        /// Specifies action to be taken when success notifier event occur.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnNotifierEventReceivedSuccess(NotifierEvent notifierEvent);
        /// <summary>
        /// Specifies action to be taken when error notifier event occur.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnNotifierEventReceivedError(NotifierEvent notifierEvent);
        /// <summary>
        /// Specifies action to be taken when notifier registration complete.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnNotifierRegistrationCompleteSuccess(NotifierEvent notifierEvent);
        /// <summary>
        /// Specifies action to be taken when notifier registration fail.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnNotifierRegistrationCompleteError(NotifierEvent notifierEvent);
    }
}

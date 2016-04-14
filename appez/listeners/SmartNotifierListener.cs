using appez.model;
using Newtonsoft.Json.Linq;

namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for listening to appez framework
    /// notifier events.
    /// </summary>
    public interface SmartNotifierListener
    {
        /// <summary>
        /// Specifies action to be taken when success notifier event occur.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnReceiveNotifierEventSuccess(NotifierEvent notifierEvent);

        /// <summary>
        /// Specifies action to be taken when error notifier event occur.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnReceiveNotifierEventError(NotifierEvent notifierEvent);

        /// <summary>
        /// Specifies action to be taken on successful registration of notifier event.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnReceiveNotifierRegistrationEventSuccess(NotifierEvent notifierEvent);

        /// <summary>
        /// Specifies action to be taken on registration error of notifier event.
        /// </summary>
        /// <param name="notifierEvent"></param>
        void OnReceiveNotifierRegistrationEventError(NotifierEvent notifierEvent);
    }
}

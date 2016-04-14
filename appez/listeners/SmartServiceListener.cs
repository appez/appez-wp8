using appez.model;

namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for listening to appez framework
    /// service completion events.
    /// </summary>
    public interface SmartServiceListener
    {
        /// <summary>
        /// Specifies action to be taken when service(s)(HTTP,UI,Camera etc.), is
        /// completed successfully
        /// </summary>
        /// <param name="smartEvent">SmartEvent object containing service completion info</param>
        void OnCompleteServiceWithSuccess(SmartEvent smartEvent);

        /// <summary>
        /// Specifies action to be taken when service(s)(HTTP,UI,Camera etc.), is
        /// completed with error
        /// </summary>
        /// <param name="smartEvent">SmartEvent object containing service completion info</param>
        void OnCompleteServiceWithError(SmartEvent smartEvent);

        void ShutDown();

    }
}

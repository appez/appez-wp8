using appez.model;

namespace appez.listeners
{
    /// <summary>
    /// Acts as a bridge between the SmartServiceListener and
    /// SmartConnectorListener for communicating notifications from service listener
    /// on completion of service operations and notify it to the
    /// SmartConnectorListener
    /// </summary>
    public interface SmartEventListener
    {
        /// <summary>
        /// Specifies action to be taken on successful completion of action. Also
        /// processes SmartEvent on the basis of event type contained in it
        /// </summary>
        /// <param name="smartEvent">SmartEvent containing event type</param>
        void OnCompleteActionWithSuccess(SmartEvent smartEvent);

        /// <summary>
        /// Specifies action to be taken on unsuccessful completion of action. Also
        /// processes SmartEvent on the basis of event type contained in it
        /// </summary>
        /// <param name="smartEvent">SmartEvent containing event type</param>
        void OnCompleteActionWithError(SmartEvent smartEvent);

        void ShutDown();

    }
}

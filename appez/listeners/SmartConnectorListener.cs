using appez.model;

namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for listening to completion
    /// status of the SmartEvent initiated by the web layer.
    /// </summary>
    public interface SmartConnectorListener
    {
        /// <summary>
        /// Specifies action to be taken on successful processing of SmartEvent. 
        /// In most cases the web layer is notified about the completion of the SmartEvent.
        /// </summary>
        /// <param name="smartEvent">SmartEvent being processed</param>
        void OnFinishProcessingWithOptions(SmartEvent smartEvent);

        /// <summary>
        /// Specifies action to be taken on unsuccessful processing of SmartEvent
        /// </summary>
        /// <param name="smartEvent">SmartEvent being processed</param>
        void OnFinishProcessingWithError(SmartEvent smartEvent);

        void ShutDown();

        /// <summary>
        /// Specifies action to be taken on receiving context specific smart
        /// notification
        /// </summary>
        /// <param name="smartEvent"></param>
        void OnReceiveContextNotification(SmartEvent smartEvent);

    }
}

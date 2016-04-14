using System;

namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for notifying the result for
    /// completion of Co-action. Essentially used by services such as Map service
    /// which lie in the co-event category
    /// </summary>
    public interface SmartCoActionListener
    {
        /// <summary>
        /// Indicates the successful completion of the co-action along with the data
        /// corresponding to the completion
        /// </summary>
        /// <param name="actionCompletionData">Data accompanying the completion of the co-event</param>
        void OnSuccessCoAction(string actionCompletionData);

        /// <summary>
        /// Indicates the erroneous completion of the co-event action
        /// </summary>
        /// <param name="exceptionType">Unique code corresponding to the problem in the co-event</param>
        /// <param name="exceptionMessage">Message describing the nature of problem with the co-event
        ///            execution</param>
        void OnErrorCoAction(int exceptionType, string exceptionMessage);
    }
}

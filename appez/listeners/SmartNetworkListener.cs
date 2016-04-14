using System;

namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for listening to network events.
    /// This includes listening to successful or erroneous completion of the HTTP
    /// calls. Based on the user preference, the response can be provided in either
    /// string format or dumped in the DAT file
    /// </summary>
    public interface SmartNetworkListener
    {
        /// <summary>
        /// Updates SmartEventResponse and thereby SmartEvent based on the HTTP
        /// operation performed in NetworkService. Also notifies SmartServiceListener
        /// about successful completion of HTTP operation
        /// </summary>
        /// <param name="responseData">HTTP response data</param>
        void OnSuccessHttpOperation(string responseData);

        /// <summary>
        /// Notifies SmartServiceListener about unsuccessful completion of HTTP
        /// operation
        /// </summary>
        /// <param name="exceptionData">Exception type</param>
        /// <param name="exceptionMessage">Message describing the type of exception</param>
        void OnErrorHttpOperation(int exceptionData, string exceptionMessage);

    }
}

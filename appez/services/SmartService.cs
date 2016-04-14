using appez.model;


namespace appez.services
{
    /// <summary>
    /// Base class of the services. All individual service classes are derived
    /// from SmartService. It exposes interface called SmartServiceListner to share
    /// processing results of service with intended client
    /// </summary>
    public abstract class SmartService
    {

        public abstract void ShutDown();

        /// <summary>
        /// Specifies action to be taken for the current service type
        /// </summary>
        /// <param name="smartEvent">SmartEvent containing parameters required for performing
        /// action in current service type</param>
        public abstract void PerformAction(SmartEvent smartEvent);


    }
}

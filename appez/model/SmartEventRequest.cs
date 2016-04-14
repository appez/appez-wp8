using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.model
{
    /// <summary>
    /// Model for holding the request parameters received from
    /// the web layer
    /// </summary>
    public class SmartEventRequest
    {
        // Unique ID corresponding to the service operation. Its a combination of 3
        // parts-Event type,Service type,Service operation type
        public int ServiceOperationId
        {
            get;
            set;
        }
        // JSONObject that contains the request data for the service
        public JObject ServiceRequestData
        {
            get;
            set;
        }
        // Indicates whether or not the service should be shutdown after the
        // completion of service operation
        public bool ServiceShutdown { get; set; }
    }
}

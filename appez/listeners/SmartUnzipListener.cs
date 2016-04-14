using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.listeners
{
    /// <summary>
    /// This listener helps listen to the successful/erroneous completion of generic
    /// events. This listener can be used for listening events that are neither of
    /// network, camera etc.
    /// </summary>
    public interface SmartUnzipListener
    {
        /// <summary>
        /// Called when the operation is completed successfully
        /// </summary>
        /// <param name="opCompData">No fixed format for sending the completion response. This
	    /// can be based on the type of event.</param>
        void OnUnzipOperationCompleteWithSuccess(string opCompData);

        /// <summary>
        /// Called when the operation could not complete successfully
        /// </summary>
        /// <param name="errorMessage"></param>
        void OnUnzipOperationCompleteWithError(string errorMessage);
    }
}

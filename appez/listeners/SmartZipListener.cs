using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.listeners
{
    /// <summary>
    /// This listener helps listen to the successful/erroneous completion of file
    /// archiving(zipping) operation.
    /// </summary>
    public interface SmartZipListener
    {
        /// <summary>
        /// Called when the operation is completed successfully
        /// </summary>
        /// <param name="opCompData">Zip operation completion response. </param>
        void OnZipOperationCompleteWithSuccess(String opCompData);

        /// <summary>
        /// Called when the zip operation could not complete successfully
        /// </summary>
        /// <param name="errorMessage">errorMessage</param>
        void OnZipOperationCompleteWithError(String errorMessage);
    }
}

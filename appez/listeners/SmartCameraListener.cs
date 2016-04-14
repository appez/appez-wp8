using System;

namespace appez.listeners
{
    /// <summary>
    /// Defines an interface for listening to Camera
    /// notifications. This includes event for sending captured image data to the web
    /// layer in case of successful/erroneous CameraService completion
    /// </summary>
    public interface SmartCameraListener
    {
        /// <summary>
        /// Specifies action to be taken when the Camera service has completed its
        /// operation. This can include capturing image from the camera or from the
        /// image gallery and applying user defined operations on it.
        /// </summary>
        /// <param name="callbackData">Well formed JSON response string that contains information
	    /// to the web layer</param>
        void OnSuccessCameraOperation(string callbackData);

        /// <summary>
        /// Specified action to be taken when the camera service has some problem
        /// completing user defined service operation
        /// </summary>
        /// <param name="exceptionData">Unique code corresponding to the problem in performing user
        /// defined camera operation</param>
        /// <param name="exceptionMessage">Message describing the nature of exception</param>
        void OnErrorCameraOperation(int exceptionData, string exceptionMessage);

    }
}

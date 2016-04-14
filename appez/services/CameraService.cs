using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.model.camera;
using appez.utility;
using Newtonsoft.Json;
using System;
using System.Text;

namespace appez.services
{
    /// <summary>
    /// Provides access to the camera hardware of the device.
    /// Supports capturing image from the camera or getting image from the gallery.
    /// Also allows the user to perform basic filter operations on the image such as
    /// Monochrome and Sepia
    /// </summary>
    public class CameraService : SmartService, SmartCameraListener
    {
        #region variables
        private CameraConfigInformation cameraConfigInformation = null;
        private SmartServiceListener smartServiceListener = null;
        private CameraUtility cameraUtility = null;
        private SmartEvent smartEvent = null;
        #endregion

        /// <summary>
        /// Creates the instance of CameraService
        /// </summary>
        /// <param name="smartServiceListener">SmartServiceListener that listens for completion events of
        /// the camera service and thereby helps notify them to the web layer</param>
        public CameraService(SmartServiceListener smartServiceListener)
        {
            this.smartServiceListener = smartServiceListener;

        }

        /// <summary>
        /// Camera service shutdown.
        /// </summary>
        public override void ShutDown()
        {
            this.smartServiceListener = null;

        }

        /// <summary>
        /// Perfrom action sets in smart event.
        /// </summary>
        /// <param name="smartEvent">Smart event</param>
        public override void PerformAction(SmartEvent smartEvent)
        {
            this.smartEvent = smartEvent;
            this.cameraUtility = new CameraUtility(this);
            InitCameraConfigInformation(this.smartEvent.SmartEventRequest.ServiceRequestData.ToString());
		    switch (smartEvent.GetServiceOperationId()) 
            {
                case WebEvents.CAMERA_LAUNCH_CAMERA:
                    if (this.cameraUtility != null)
                    {
                        // Launch camera
                        this.cameraUtility.LaunchCamera(this.cameraConfigInformation);
                    }
                    break;

                case WebEvents.CAMERA_LAUNCH_GALLERY:
                    if (this.cameraUtility != null)
                    {
                        // Launch image gallary
                        this.cameraUtility.LaunchGallery(this.cameraConfigInformation);
                    }
                    break;

                default:

                    break;
            }
        }

        /// <summary>
        /// SmartCameraListener method that listens for successful completion of
        /// camera service operation. Responsible for creating the SmartEventResponse
        /// and dispatching it to the corresponding SmartServiceListener
        /// </summary>
        /// <param name="fileName">File path saved in isolated storage</param>
        /// <param name="responseData">Image as base64</param>
        public void OnSuccessCameraOperation(String callbackData)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete=true;
            smEventResponse.ServiceResponse=callbackData;
            smEventResponse.ExceptionType=0;
            smEventResponse.ExceptionMessage=null;
            smartEvent.SmartEventResponse=smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }

        /// <summary>
        /// SmartCameraListener method that listens for unsuccessful completion of
        /// camera service operation. Responsible for creating the SmartEventResponse
        /// and dispatching it to the corresponding SmartServiceListener
        /// </summary>
        /// <param name="exceptionData">Unique code corresponding to the error in performing camera operation</param>
        /// <param name="exceptionMessage">Message describing the problem in executing the request</param>
        public void OnErrorCameraOperation(int exceptionType, String exceptionMessage)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = false;
            smEventResponse.ServiceResponse=null;
            smEventResponse.ExceptionType=exceptionType;
            smEventResponse.ExceptionMessage=exceptionMessage;
            smartEvent.SmartEventResponse=smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(smartEvent);
        }
        /// <summary>
        /// Prepares the <see cref="CameraConfigInformation"/> model from the user provided
	    /// camera service information
        /// </summary>
        /// <param name="cameraInfo"></param>
        private void InitCameraConfigInformation(String cameraInfo)
        {
            this.cameraConfigInformation = this.ParseCameraConfigInfo(cameraInfo, smartEvent.GetServiceOperationId());
        }
        /// <summary>
        /// Parse camera config information json into cameraConfiginformation object.
        /// </summary>
        /// <param name="configInfo">Camera Configuration Information</param>
        /// <param name="cameraCaptureType">Indicates source of image camera/gallery.</param>
        protected CameraConfigInformation ParseCameraConfigInfo(string configInfo, int cameraCaptureType)
        {
            try
            {
                if (configInfo != null)
                {
                    CameraConfigInformation cameraConfiginformation = null;
                    // Deserialize camera config information json into cameraConfiginformaiton class.
                    cameraConfiginformation = JsonConvert.DeserializeObject<CameraConfigInformation>(configInfo);

                    cameraConfiginformation.source = cameraCaptureType;
                    return cameraConfiginformation;

                }
                else
                {
                    throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
                }

            }
            catch (JsonReaderException jsonReaderException)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred while parsing camera config information : " + jsonReaderException.Message.ToString());
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }

        }

    }
}

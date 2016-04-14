using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using Windows.System;

namespace appez.services
{
    public class LocationService : SmartService, DialogListener
    {

        private SmartEvent smartEvent = null;
        private SmartServiceListener smartServiceListener = null;

        private Geolocator locManager = null;

        private string locationAccuracy = null;
        private bool isLastKnownAllowed = false;

        private readonly uint ACCURACY_FINE = 50;
        private readonly uint ACCURACY_COARSE = 500;


        private UIUtility mDialogBuilder = null;

        // Check the request parameters provided from the web client

        private int locationRequestTimeout = 0;
        
        /// <summary>
        /// Creates the instance of LocationService
        /// </summary>
        /// <param name="smartServiceListener">SmartServiceListener that listens for completion events of
        /// the location service and thereby helps notify them to the web
        /// layer</param>
        public LocationService(SmartServiceListener smartServiceListener)
        {
            this.smartServiceListener = smartServiceListener;
        }


        public override void ShutDown()
        {
            this.smartServiceListener = null;
        }

        public override void PerformAction(SmartEvent smartEvent)
        {

            this.smartEvent = smartEvent;
            ProcessLocationRequest(smartEvent);
            switch (smartEvent.GetServiceOperationId())
            {
                case WebEvents.WEB_USER_CURRENT_LOCATION:
                    GetCurrentLocation();
                    break;

            }
        }

        /// <summary>
        /// Parses the request coming from the web layer to get user preferences.
        /// </summary>
        /// <param name="smartEvent"></param>
        private void ProcessLocationRequest(SmartEvent smartEvent)
        {
            try
            {
                JObject serviceRequestData = smartEvent.SmartEventRequest.ServiceRequestData;
                JToken tempToken = null;
                if (serviceRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_LOC_ACCURACY, out tempToken))
                {
                    this.locationAccuracy = tempToken.ToString();
                    
                }
                if (serviceRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_LOCATION_LASTKNOWN, out tempToken))
                {
                    this.isLastKnownAllowed = Convert.ToBoolean(tempToken.ToString());
                }
                if (serviceRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_LOCATION_TIMEOUT, out tempToken))
                {
                    this.locationRequestTimeout = Convert.ToInt32(tempToken.ToString());
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            catch (InvalidCastException)
            {
                throw new MobiletException(ExceptionTypes.INVALID_SERVICE_REQUEST_ERROR);
            }
        }

        /// <summary>
        /// Find user current location using geolocation.
        /// </summary>
        private async void GetCurrentLocation()
        {
            this.locManager = new Geolocator();
            if (this.locationAccuracy == SmartConstants.LOCATION_ACCURACY_FINE)
            {
                this.locManager.DesiredAccuracyInMeters = ACCURACY_FINE;
            }
            else
            {
                this.locManager.DesiredAccuracyInMeters = ACCURACY_COARSE;
            }
            ShowProgressDialog();
            try
            {
                // Request the current position
                Geoposition geoposition = await this.locManager.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(this.locationRequestTimeout)
                    );

                String locationResponse = LocationUtility.PrepareLocationResponse(geoposition.Coordinate);
                OnSuccessLocationOperation(locationResponse);
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location is off
                    ShowDecisionDialog("Location Disabled", "Location is disabled in your device. Enable it?");
                }
                else
                {
                    OnErrorLocationOperation(ExceptionTypes.ERROR_RETRIEVING_CURRENT_LOCATION, ExceptionTypes.ERROR_RETRIEVING_CURRENT_LOCATION_MESSAGE);
                }
            }

        }
        /// <summary>
        /// show loading indicator.
        /// </summary>
        private void ShowProgressDialog()
        {
            if (mDialogBuilder == null)
            {
                mDialogBuilder = new UIUtility(this);
            }
            mDialogBuilder.ShowProgressBarWithMessage("Determining current location...");

        }
        /// <summary>
        /// hide loading indicator.
        /// </summary>
        private void HideProgressDialog()
        {
            mDialogBuilder.HideActivityView();
            mDialogBuilder = null;
        }
        /// <summary>
        /// show decision diaglog to turn location service on.
        /// </summary>
        /// <param name="dialogTitle"></param>
        /// <param name="dialogMessage"></param>
        private void ShowDecisionDialog(String dialogTitle, String dialogMessage)
        {
            if (mDialogBuilder == null)
            {
                mDialogBuilder = new UIUtility(this);
            }
            mDialogBuilder.CreateDialog(WebEvents.WEB_SHOW_MESSAGE_YESNO, dialogTitle + SmartConstants.MESSAGE_DIALOG_TITLE_TEXT_SEPARATOR + dialogMessage);
        }
        public async void ProcessUsersSelection(string userSelection)
        {
            
            if (userSelection.Equals(SmartConstants.USER_SELECTION_YES))
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
            }
            OnErrorLocationOperation(ExceptionTypes.LOCATION_ERROR_GPS_NETWORK_DISABLED, ExceptionTypes.LOCATION_ERROR_GPS_NETWORK_DISABLED_MESSAGE);
        }
        /// <summary>
        /// Sends the response for successful completion of location operation
        /// </summary>
        /// <param name="callbackData"></param>
        private void OnSuccessLocationOperation(String callbackData)
        {
            
            if (mDialogBuilder != null)
            {
                HideProgressDialog();
            }
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = true;
            smEventResponse.ServiceResponse = callbackData;
            smEventResponse.ExceptionType = 0;
            smEventResponse.ExceptionMessage = null;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }

        /// <summary>
        /// Sends the response for unsuccessful completion of location operation
        /// </summary>
        /// <param name="exceptionType"></param>
        /// <param name="exceptionMessage"></param>
        private void OnErrorLocationOperation(int exceptionType, String exceptionMessage)
        {
            
            if (mDialogBuilder != null)
            {
                HideProgressDialog();
            }

            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = false;
            smEventResponse.ServiceResponse = null;
            smEventResponse.ExceptionType = exceptionType;
            smEventResponse.ExceptionMessage = exceptionMessage;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(smartEvent);
        }
    }
}

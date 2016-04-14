using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.utility;
using appez.utility.uicontrols.contentpicker;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Windows;

namespace appez.services
{
    /// <summary>
    /// UIService which extends SmartService and implements
    /// SmartPickerListener
    /// </summary>
    public class UIService : SmartService, DialogListener
    {
        #region variables
        private SmartServiceListener smartServiceListener = null;
        private SmartEvent currentEvent = null;
        private UIUtility uiUtility = null;
        private String uiServiceResponse = null;
        
        #endregion
        /// <summary>
        /// Creates the instance of UIService
        /// </summary>
        /// <param name="smartServiceListener">SmartServiceListener</param>
        public UIService(SmartServiceListener smartServiceListener)
        {
            this.AttachHardwareButtonHandlers();
            this.smartServiceListener = smartServiceListener;
            this.uiUtility = new UIUtility(this);
        }


        public override void ShutDown()
        {

            this.smartServiceListener = null;
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                if (page != null)
                {
                    page.BackKeyPress -= Page_BackKeyPress;

                }
            }

        }

        /// <summary>
        /// Performs UI action based on SmartEvent action type
        /// </summary>
        /// <param name="smartEvent">SmartEvent specifying action type for the UI action</param>
        public override void PerformAction(SmartEvent smartEvent)
        {
            try
            {
                String message = smartEvent.SmartEventRequest.ServiceRequestData.GetValue(CommMessageConstants.MMI_REQUEST_PROP_MESSAGE).ToString();

                JObject activityIndicatorResponse = new JObject();
                switch (smartEvent.GetServiceOperationId())
                {
                    case WebEvents.WEB_SHOW_ACTIVITY_INDICATOR:
                        {
                            StartLoading(message);
                            activityIndicatorResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTION, "null");
                            uiServiceResponse = activityIndicatorResponse.ToString();
                            OnSuccessUiOperation(smartEvent);
                        }
                        break;

                    case WebEvents.WEB_HIDE_ACTIVITY_INDICATOR:
                        {
                            HideProgressDialog();
                            activityIndicatorResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTION, "null");
                            uiServiceResponse = activityIndicatorResponse.ToString();
                            OnSuccessUiOperation(smartEvent);
                        }
                        break;
                    case WebEvents.WEB_SHOW_INDICATOR:
                        {
                            uiUtility.HideIndicator();
                            uiUtility.ShowIndicator();
                            activityIndicatorResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTION, "null");
                            uiServiceResponse = activityIndicatorResponse.ToString();
                            OnSuccessUiOperation(smartEvent);

                        }
                        break;

                    case WebEvents.WEB_HIDE_INDICATOR:
                        {
                            uiUtility.HideIndicator();
                            activityIndicatorResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTION, "null");
                            uiServiceResponse = activityIndicatorResponse.ToString();
                            OnSuccessUiOperation(smartEvent);

                        }
                        break;

                    case WebEvents.WEB_UPDATE_LOADING_MESSAGE:
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    UpdateProcessDialogMessage(message);
                    activityIndicatorResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTION, "null");
                    uiServiceResponse = activityIndicatorResponse.ToString();
                    OnSuccessUiOperation(smartEvent);
                });
                        break;

                    case WebEvents.WEB_SHOW_MESSAGE:
                        if (message == null || message.Length == 0 || message.Equals("null"))
                        {
                            message = ExceptionTypes.UNABLE_TO_PROCESS_MESSAGE;
                        }
                        this.currentEvent = smartEvent;
                        CreateDialog(WebEvents.WEB_SHOW_MESSAGE, message);

                        break;

                    case WebEvents.WEB_SHOW_MESSAGE_YESNO:
                        if (message == null || message.Length == 0 || message.Equals("null"))
                        {
                            message = ExceptionTypes.UNABLE_TO_PROCESS_MESSAGE;
                        }
                        this.currentEvent = smartEvent;
                        CreateDialog(WebEvents.WEB_SHOW_MESSAGE_YESNO, message);
                        break;

                    case WebEvents.WEB_SHOW_DATE_PICKER:
                        this.currentEvent = smartEvent;
                        CreateDateSelector();
                        break;

                    case WebEvents.WEB_SHOW_DIALOG_SINGLE_CHOICE_LIST:
                        if (message == null || message.Length == 0 || message.Equals("null"))
                        {
                            message = ExceptionTypes.UNABLE_TO_PROCESS_MESSAGE;
                        }
                        this.currentEvent = smartEvent;
                        SmartMessagePickerView smartMessagePickerView = new SmartMessagePickerView(message, "Normal", this);
                        uiUtility.ShowChildPopup(smartMessagePickerView);
                        break;

                    case WebEvents.WEB_SHOW_DIALOG_SINGLE_CHOICE_LIST_RADIO_BTN:
                        if (message == null || message.Length == 0 || message.Equals("null"))
                        {
                            message = ExceptionTypes.UNABLE_TO_PROCESS_MESSAGE;
                        }
                        this.currentEvent = smartEvent;
                        SmartMessagePickerView smartRadioMessagePicker = new SmartMessagePickerView(message, "Radio", this);
                        uiUtility.ShowChildPopup(smartRadioMessagePicker);
                        break;

                    case WebEvents.WEB_SHOW_DIALOG_MULTIPLE_CHOICE_LIST_CHECKBOXES:
                        if (message == null || message.Length == 0 || message.Equals("null"))
                        {
                            message = ExceptionTypes.UNABLE_TO_PROCESS_MESSAGE;
                        }
                        this.currentEvent = smartEvent;
                        SmartMessagePickerView smartCheckboxMessagePicker = new SmartMessagePickerView(message, "Checkbox", this);
                        uiUtility.ShowChildPopup(smartCheckboxMessagePicker);
                        break;

                }
            }
            catch (JsonException)
            {
                OnErrorUiOperation(ExceptionTypes.JSON_PARSE_EXCEPTION, ExceptionTypes.JSON_PARSE_EXCEPTION_MESSAGE);
            }
            catch (Exception ex)
            {
                OnErrorUiOperation(ExceptionTypes.UNKNOWN_EXCEPTION, ex.Message);
            }

        }

        private void HideProgressDialog()
        {
            uiUtility.HideActivityView();
        }

        private void UpdateProcessDialogMessage(String updatedMessage)
        {
            uiUtility.SetProgressBarMessage(updatedMessage);
        }

        private void StartLoading(String mStrStatusMessage)
        {
            uiUtility.ShowProgressBarWithMessage(mStrStatusMessage);
        }

        private void CreateDialog(int id, String message)
        {
            uiUtility.CreateDialog(id, message);

        }
        
        private void CreateDateSelector()
        {
            uiUtility.CreateDatePicker();
        }

        public void ProcessUsersSelection(string userSelection)
        {

            try
            {
                JObject activityIndicatorResponse = new JObject();
                activityIndicatorResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTION, userSelection);
                uiServiceResponse = activityIndicatorResponse.ToString();
                OnSuccessUiOperation(currentEvent);
            }
            catch (JsonException)
            {
                OnErrorUiOperation(ExceptionTypes.JSON_PARSE_EXCEPTION, ExceptionTypes.JSON_PARSE_EXCEPTION_MESSAGE);
            }

        }

        /// <summary>
        /// Adds hardware back key press event to current page.
        /// </summary>
        void AttachHardwareButtonHandlers()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                if (page != null)
                {
                    page.BackKeyPress += new EventHandler<CancelEventArgs>(Page_BackKeyPress);

                }
            }
        }
        /// <summary>
        /// Handle hardware back key press event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_BackKeyPress(object sender, CancelEventArgs e)
        {

            try
            {
                uiUtility.HideChildPopup();
                ProcessUsersSelection(SmartConstants.USER_SELECTION_CANCEL);
                e.Cancel = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while invoking backbutton into view: " + ex.Message);
            }


        }
        /// <summary>
        /// Responsible for preparing the successful response callback on the
        /// completion of the <see cref="UIService"/> operation
        /// </summary>
        /// <param name="smartEvent">SmartEvent object that will be modified to add the SmartEventResponse</param>
        public void OnSuccessUiOperation(SmartEvent smartEvent)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = true;
            smEventResponse.ServiceResponse = uiServiceResponse;
            smEventResponse.ExceptionType = 0;
            smEventResponse.ExceptionMessage = null;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }
        /// <summary>
        /// Responsible for preparing the error response callback on the
        /// completion of the <see cref="UIService"/> operation
        /// </summary>
        /// <param name="exceptionType">Exception type</param>
        /// <param name="exceptionMessage">Exception message</param>
        public void OnErrorUiOperation(int exceptionType, String exceptionMessage)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = false;
            smEventResponse.ServiceResponse = null;
            smEventResponse.ExceptionType = exceptionType;
            smEventResponse.ExceptionMessage = exceptionMessage;
            currentEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(currentEvent);
        }
    }
}

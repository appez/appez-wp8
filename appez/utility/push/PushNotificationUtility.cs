using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.listeners.notifier;
using appez.model;
using Microsoft.Phone.Notification;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace appez.utility.push
{
    /// <summary>
    /// Utility for handle push notification events.
    /// </summary>
    public class PushNotificationUtility : SmartNetworkListener, SmartPushListener
    {

        private NotifierPushMessageListener notifierPushMessageListener = null;
        /// Holds the push channel that is created or found.
        HttpNotificationChannel pushChannel;

        private static String PUSH_SERVER_URL = null;
        private static String PUSH_LOADING_MSG = null;

        private NotifierEvent currentNotifierEvent = null;

        private int currentEvent = -1;
        private const int CURRENT_EVENT_PUSH_REGISTER = 0;
        private const int CURRENT_EVENT_PUSH_UNREGISTER = 1;


        public PushNotificationUtility()
        {

        }

        public PushNotificationUtility(NotifierPushMessageListener smPushListener)
        {
            this.notifierPushMessageListener = smPushListener;
        }


        public void Register(NotifierEvent notifierEvent)
        {
            this.currentEvent = CURRENT_EVENT_PUSH_REGISTER;
            InitRegistrationParams(notifierEvent);
            this.currentNotifierEvent = notifierEvent;
            // The name of our push channel.
            string channelName = AppUtility.GetApplicationName();
            try
            {


                // Try to find the push channel.
                pushChannel = HttpNotificationChannel.Find(channelName);

                // If the channel was not found, then create a new connection to the push service.
                if (pushChannel == null)
                {
                    pushChannel = new HttpNotificationChannel(channelName);

                    // Register for all the events before attempting to open the channel.
                    pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                    pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                    // Register for this notification only if you need to receive the notifications while your application is running.
                    pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                    pushChannel.Open();

                    // Bind this new channel for toast events.
                    pushChannel.BindToShellToast();

                }
                else
                {
                    // The channel was already open, so just register for all the events.
                    pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                    pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                    // Register for this notification only if you need to receive the notifications while your application is running.
                    pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                    // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                    System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());

                }

            }
            catch (UnauthorizedAccessException)
            {
                throw new MobiletException(ExceptionTypes.NOTIFIER_REQUEST_ERROR);
            }
            catch (Exception)
            {
                throw new MobiletException(ExceptionTypes.NOTIFIER_REQUEST_ERROR);
            }
        }

        /// <summary>
        /// Event handler for when the push channel Uri is updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
            RegisterDeviceWithServer(e.ChannelUri.ToString());
        }

        /// <summary>
        /// Event handler for when a push notification error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            // Error handling logic for your particular application would be here.
            OnError(e.Message.ToString());
        }

        /// <summary>
        /// Event handler for when a toast notification arrives while your application is running.  
        /// The toast will not display if your application is running so you must add this
        /// event handler if you want to do something with the toast notification.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }
            // From here message will propagate to js.
            OnMessage(message.ToString());
            // When the application is not in forground then notification will be at
            // Navigated event of page specified at the time of push notification at
            // server. 


        }
        /// <summary>
        ///  Unregister this account/device pair within the server.
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void Unregister(NotifierEvent notifierEvent)
        {
            this.currentEvent = CURRENT_EVENT_PUSH_UNREGISTER;
            InitUnregistrationParams(notifierEvent);
            this.currentNotifierEvent = notifierEvent;
            UnregisterDeviceWithServer("");
            if (pushChannel != null)
            {
                pushChannel.Close();
                pushChannel.Dispose();
            }

        }

        /// <summary>
        /// Prepare notifier event response obejct on success.
        /// </summary>
        /// <param name="notifierResponse"></param>
        /// <returns></returns>
        private NotifierEvent PrepareNotifierEventResponseSuccess(JObject notifierResponse)
        {
            NotifierEvent notifierEvent = new NotifierEvent();
            notifierEvent.TransactionId = DateTime.Now.ToString("yyyyMMddHHmmss");
            notifierEvent.Type = NotifierConstants.PUSH_MESSAGE_NOTIFIER;
            notifierEvent.IsOperationSuccess = true;
            notifierEvent.EventResponse = notifierResponse;
            notifierEvent.ErrorType = 0;
            notifierEvent.ErrorMessage = null;

            return notifierEvent;

        }

        /// <summary>
        /// Prepare notifier event response on error.
        /// </summary>
        /// <param name="notifierError"></param>
        /// <returns></returns>
        private NotifierEvent PrepareNotifierEventResponseError(String notifierError)
        {

            NotifierEvent notifierEvent = new NotifierEvent();
            notifierEvent.TransactionId = DateTime.Now.ToString("yyyyMMddHHmmss");
            notifierEvent.Type = NotifierConstants.PUSH_MESSAGE_NOTIFIER;
            notifierEvent.IsOperationSuccess = false;
            notifierEvent.EventResponse = new JObject();
            notifierEvent.ErrorType = ExceptionTypes.NOTIFIER_REQUEST_ERROR;
            notifierEvent.ErrorMessage = notifierError;

            return notifierEvent;

        }
        /// <summary>
        /// Modifies existing NotifierEvent to add required parameters for
        /// response
        /// </summary>
        /// <param name="notifierEvent"></param>
        /// <param name="notifierResponse"></param>
        /// <returns></returns>
        private NotifierEvent PrepareResponseFromCurrentEventSuccess(NotifierEvent notifierEvent, JObject notifierResponse)
        {
            if (notifierEvent != null)
            {
                notifierEvent.IsOperationSuccess = true;
                notifierEvent.EventResponse = notifierResponse;
                notifierEvent.ErrorType = 0;
                notifierEvent.ErrorMessage = null;
            }
            return notifierEvent;
        }
        /// <summary>
        /// Modifies existing NotifierEvent to add required parameters for
        /// response
        /// </summary>
        /// <param name="notifierEvent"></param>
        /// <param name="notifierError"></param>
        /// <returns></returns>
        private NotifierEvent PrepareResponseFromCurrentEventError(NotifierEvent notifierEvent, String notifierError)
        {
            if (notifierEvent != null)
            {
                notifierEvent.IsOperationSuccess = false;
                notifierEvent.EventResponse = null;
                notifierEvent.ErrorType = ExceptionTypes.NOTIFIER_REQUEST_ERROR;
                notifierEvent.ErrorMessage = notifierError;
            }
            return notifierEvent;
        }
        /// <summary>
        /// Once the registration ID has been received from the MPNS, register that
        /// device with UPS server
        /// </summary>
        /// <param name="registrationId"></param>
        private void RegisterDeviceWithServer(String registrationId)
        {
            try
            {
                JObject networkReqObj = new JObject();
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_URL, PUSH_SERVER_URL);
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_METHOD, SmartConstants.HTTP_REQUEST_TYPE_POST);
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_CONTENT_TYPE, "application/json");

                // Prepare the Post body of the request
                JObject requestPostBody = new JObject();
                // Here we are using Device IMEI as device ID
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_DEVICEID, AppUtility.GetDeviceImei());
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_APPID, AppUtility.GetApplicationName());
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_APPNAME, AppUtility.GetApplicationName());
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_APPVERSION, AppUtility.GetApplicationVersion());
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_PLATFORM, "WP");
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_PUSHID, registrationId);
                String requestBodyString = requestPostBody.ToString();
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_POST_BODY, requestBodyString);
                Debug.WriteLine("PushNotificationUtility->registerDeviceWithServer->networkReqObj:" + networkReqObj.ToString());

                this.currentEvent = CURRENT_EVENT_PUSH_REGISTER;
                if (IsNetworkReachable())
                {
                    HttpUtility httpUtility = new HttpUtility(this);
                    httpUtility.PerformAsyncRequest(networkReqObj.ToString(), false);
                }
            }
            catch (JsonException je)
            {
                notifierPushMessageListener.OnPushRegistrationCompleteError(PrepareResponseFromCurrentEventError(currentNotifierEvent, je.Message));
            }
            catch (Exception e)
            {
                notifierPushMessageListener.OnPushRegistrationCompleteError(PrepareResponseFromCurrentEventError(currentNotifierEvent, e.Message));
            }
        }

        /// <summary>
        /// Once the device has unregistered from MPNS, unregister that device with
        /// UPS server also
        /// </summary>
        /// <param name="registrationId"></param>
        private void UnregisterDeviceWithServer(String registrationId)
        {
            try
            {
                JObject networkReqObj = new JObject();
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_URL, PUSH_SERVER_URL);
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_METHOD, SmartConstants.HTTP_REQUEST_TYPE_POST);
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_CONTENT_TYPE, "application/json");

                // Prepare the Post body of the request
                JObject requestPostBody = new JObject();
                // Here we are using Device IMEI as device ID
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_DEVICEID, AppUtility.GetDeviceImei());
                requestPostBody.Add(NotifierConstants.PUSH_REGISTER_REQ_PROP_APPID, AppUtility.GetApplicationName());
                String requestBodyString = requestPostBody.ToString();
                networkReqObj.Add(CommMessageConstants.MMI_REQUEST_PROP_REQ_POST_BODY, requestBodyString);
                Debug.WriteLine("PushNotificationUtility->unregisterDeviceWithServer->networkReqObj:" + networkReqObj.ToString());

                this.currentEvent = CURRENT_EVENT_PUSH_UNREGISTER;
                if (IsNetworkReachable())
                {
                    HttpUtility httpUtility = new HttpUtility(this);
                    httpUtility.PerformAsyncRequest(networkReqObj.ToString(), false);
                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }
        /// <summary>
        /// Handles http request success.
        /// </summary>
        /// <param name="responseData"></param>
        public void OnSuccessHttpOperation(string responseData)
        {
            Debug.WriteLine("PushNotificationUtility->onSuccessHttpOperation->current event:" + this.currentEvent + "response data:" + responseData);
            switch (currentEvent)
            {
                case CURRENT_EVENT_PUSH_REGISTER:
                    notifierPushMessageListener.OnPushRegistrationCompleteSuccess(PrepareResponseFromCurrentEventSuccess(currentNotifierEvent, new JObject()));

                    break;

                case CURRENT_EVENT_PUSH_UNREGISTER:
                    notifierPushMessageListener.OnPushRegistrationCompleteSuccess(PrepareResponseFromCurrentEventSuccess(currentNotifierEvent, new JObject()));

                    break;
            }
        }
        /// <summary>
        /// Handles http request error.
        /// </summary>
        /// <param name="exceptionData"></param>
        /// <param name="exceptionMessage"></param>
        public void OnErrorHttpOperation(int exceptionData, string exceptionMessage)
        {
            Debug.WriteLine("PushNotificationUtility->onErrorHttpOperation->exceptionData:" + exceptionData + ",exceptionMessage:" + exceptionMessage);
            switch (currentEvent)
            {
                case CURRENT_EVENT_PUSH_REGISTER:
                    notifierPushMessageListener.OnPushRegistrationCompleteError(PrepareResponseFromCurrentEventError(currentNotifierEvent, exceptionMessage));
                    break;

                case CURRENT_EVENT_PUSH_UNREGISTER:
                    notifierPushMessageListener.OnPushRegistrationCompleteError(PrepareResponseFromCurrentEventError(currentNotifierEvent, exceptionMessage));
                    break;
            }
        }
        /// <summary>
        /// Checks whether or not the network is reachable
        /// </summary>
        /// <returns></returns>
        private bool IsNetworkReachable()
        {
            bool isNetworkReachable = false;
            NetworkReachabilityUtility nwReachability = NetworkReachabilityUtility.GetInstance();
            isNetworkReachable = nwReachability.CheckForConnection();

            return isNetworkReachable;
        }

        public void OnMessage(String pushNotification)
        {

            // From this method the message will be propogated to the JS layer
            try
            {
                NotifierEvent notifierEventResponse = new NotifierEvent();
                notifierEventResponse.Type = NotifierConstants.PUSH_MESSAGE_NOTIFIER;
                notifierPushMessageListener.OnPushEventReceivedSuccess(notifierEventResponse);
            }
            catch (JsonException)
            {
                notifierPushMessageListener.OnPushEventReceivedError(new NotifierEvent());
            }
        }


        public void OnDeletedMessages(int total)
        {

        }

        public void OnError(string errorMessage)
        {
            notifierPushMessageListener.OnPushEventReceivedError(PrepareNotifierEventResponseError(ExceptionTypes.NOTIFIER_REQUEST_ERROR_MESSAGE));

        }

        public void OnRecoverableError(string errorId)
        {

        }
        private void InitRegistrationParams(NotifierEvent notifierEvent)
        {
            try
            {
                JObject registrationParams = notifierEvent.RequestData;

                if (registrationParams != null)
                {
                    JToken tempToken;


                    if (registrationParams.TryGetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_SERVER_URL, out tempToken))
                    {
                        PUSH_SERVER_URL = registrationParams.GetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_SERVER_URL).ToString();
                    }

                    if (registrationParams.TryGetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_LOADING_MESSAGE, out tempToken))
                    {
                        PUSH_LOADING_MSG = registrationParams.GetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_LOADING_MESSAGE).ToString();
                    }


                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }

        private void InitUnregistrationParams(NotifierEvent notifierEvent)
        {
            try
            {
                JObject unregistrationParams = notifierEvent.RequestData;

                if (unregistrationParams != null)
                {
                    JToken tempToken;
                    if (unregistrationParams.TryGetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_SERVER_URL, out tempToken))
                    {
                        PUSH_SERVER_URL = unregistrationParams.GetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_SERVER_URL).ToString();
                    }

                    if (unregistrationParams.TryGetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_LOADING_MESSAGE, out tempToken))
                    {
                        PUSH_LOADING_MSG = unregistrationParams.GetValue(NotifierMessageConstants.NOTIFIER_PUSH_PROP_LOADING_MESSAGE).ToString();
                    }
                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }

    }
}

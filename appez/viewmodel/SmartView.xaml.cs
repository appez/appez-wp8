using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using appez.listeners;
using appez.exceptions;
using appez.constants;
using appez.model;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.IO;
using appez.utility;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace appez
{
    public partial class SmartView : UserControl, SmartConnectorListener, SmartNotificationListener, SmartNotifierListener
    {
        #region variables
        MobiletManager smartConnector = null;
        private static string JS_HANDLE_NATIVE_EXCEPTION = "";
        private static string JS_NOTIFICATION_FROM_NATIVE = "appez.mmi.manager.MobiletManager.notificationFromNative";
        string htmlUri = null;
        bool hideSplash = false;
        JObject appConfigInformation = null;
        JArray appMenuInformation = null;
        #endregion
        /// <summary>
        ///  WebBrowser page url to load. Setting StartPageUri only has an effect if called before the view is loaded.
        /// </summary>
        public string StartPageUri
        {
            get
            {
                if (htmlUri == null)
                {
                    // default
                    return "Assets/www/app/smartphone/html/index.html";
                }
                else
                {
                    return htmlUri;
                }
            }
            set
            {
                htmlUri = value;

            }
        }

        public SmartView()
        {
            InitializeComponent();

            StartupMode mode = PhoneApplicationService.Current.StartupMode;
            // Initialise Smart connector here with reference of
            // SmartViewActivity to receive SmartConnectorListener
            // notifications
            this.smartConnector = new MobiletManager(this);

            if (mode == StartupMode.Launch)
            {
                PhoneApplicationService service = PhoneApplicationService.Current;
                service.Activated += new EventHandler<Microsoft.Phone.Shell.ActivatedEventArgs>(AppActivated);
                service.Launching += new EventHandler<LaunchingEventArgs>(AppLaunching);
                service.Deactivated += new EventHandler<DeactivatedEventArgs>(AppDeactivated);
                service.Closing += new EventHandler<ClosingEventArgs>(AppClosing);
            }
            else
            {

            }
            // Check for the presence of the application configuration information
            InitAppConfigInformation();

            this.ToggleSplashScreen();
            this.StartPageUri = AppUtility.GetStringForId("page_url");


        }

        private void InitAppConfigInformation()
        {
            try
            {
                string configFileLocation = "www/app/appez.conf";
                appConfigInformation = AppUtility.GetAppConfigFileProps(configFileLocation);
                JToken tempToken;
                // Set the application menu information provided in the
                // 'appez.conf' file
                if (appConfigInformation.TryGetValue(SmartConstants.APPEZ_CONF_PROP_MENU_INFO, out tempToken))
                {
                    appMenuInformation = new JArray(appConfigInformation.GetValue(SmartConstants.APPEZ_CONF_PROP_MENU_INFO).ToString());
                }
                else
                {
                    appMenuInformation = new JArray();
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }

        #region Application life cycle methods
        void AppClosing(object sender, ClosingEventArgs e)
        {
            Debug.WriteLine("AppClosing");
        }

        void AppDeactivated(object sender, DeactivatedEventArgs e)
        {
            Debug.WriteLine("INFO: AppDeactivated");

            try
            {

            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Pause event error");
            }
        }

        void AppLaunching(object sender, LaunchingEventArgs e)
        {
            Debug.WriteLine("INFO: AppLaunching");
        }

        void AppActivated(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            Debug.WriteLine("INFO: AppActivated");
            try
            {

            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR: Resume event error");
            }
        }

        #endregion

        /// <summary>
        /// Got focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewAppear(object sender, System.Windows.RoutedEventArgs e)
        {
            StartupMode mode = PhoneApplicationService.Current.StartupMode;
            if (!(mode == StartupMode.Launch))
            {
                // TODO: Add event handler implementation here.
				Debug.WriteLine("viewAppear");
            }

        }

        /// <summary>
        /// Lost focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewDisAppear(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Debug.WriteLine("viewDisAppear");
        }

        #region WebBrowser Events
        private void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("viewDisAppear");

        }

        private void WebBrowser_Navigating(object sender, NavigatingEventArgs e)
        {
            Debug.WriteLine("viewDisAppear");
        }

        private void WebBrowser_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Debug.WriteLine("viewDisAppear");
        }

        private void WebBrowser_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("viewDisAppear");
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {

            Debug.WriteLine("WebBrowser loaded : viewDisAppear");
            //attach back buuton handler
            AttachHardwareButtonHandlers();

            if (this.StartPageUri != null && this.StartPageUri != "")
            {

                //Check if current source of webview is null or not equal to htmluri, then navigate to html uri.
                if ((this.webView.Source == null) || (!this.webView.Source.OriginalString.Contains(this.StartPageUri)))
                {
                    this.webView.Navigate(new Uri(this.StartPageUri, UriKind.RelativeOrAbsolute));
                }

            }
            else
            {
                throw new MobiletException(ExceptionTypes.INVALID_PAGE_URI_EXCEPTION);
            }

        }

        void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this.hideSplash = true;
            this.ToggleSplashScreen();

        }

        #endregion

        /// <summary>
        ///  Specifies action to be taken for PAGE_INIT_NOTIFICATION
        /// </summary>
        public void NotifyPageInit()
        {
            string notificationMethod = JS_NOTIFICATION_FROM_NATIVE;
            string notificationArgument = "" + SmartConstants.NATIVE_EVENT_PAGE_INIT_NOTIFICATION;
            string notifyJavaScript = this.NotifyToJavaScript(notificationMethod, notificationArgument);
            this.ExecuteJavaScript(notifyJavaScript);
        }
        /// <summary>
        /// Show/hide splashscreen.
        /// </summary>
        public async void ToggleSplashScreen()
        {
            if (splashImageView != null)
            {
                if (!hideSplash)
                {

                    splashImageView.Visibility = Visibility.Visible;
                    webView.Visibility = Visibility.Collapsed;
                    splashImageView.Source = this.DefaultImage();
                }
                else
                {
                    webView.Visibility = Visibility.Visible;
                    // TODO : identify alternative of delay.
                    await Task.Delay(400);
                    splashImageView.Visibility = Visibility.Collapsed;

                }
            }
        }

        /// <summary>
        /// Create Bitmap image from resource.
        /// </summary>
        /// <returns>BitmapImage</returns>
        public BitmapImage DefaultImage()
        {
            BitmapImage image = new BitmapImage();
            try
            {

                var resourcePath = Application.GetResourceStream(new Uri("Assets/background.png", UriKind.Relative));
                StreamReader streamReader = new StreamReader(resourcePath.Stream);

                image.SetSource(streamReader.BaseStream);

            }
            catch (System.IO.FileNotFoundException)
            {

            }
            return image;
        }


        /// <summary>
        /// Native Hardware events
        /// </summary>
        void AttachHardwareButtonHandlers()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                if (page != null)
                {
                    // add event handler for back key. 
                    page.BackKeyPress -= Page_BackKeyPress;
                    page.BackKeyPress += Page_BackKeyPress;
                    page.NavigationService.Navigating -= NavigationService_Navigating;
                    page.NavigationService.Navigating += NavigationService_Navigating;
                    page.NavigationService.Navigated -= NavigationService_Navigated;
                    page.NavigationService.Navigated += NavigationService_Navigated;
                }
            }
        }

        void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("Naivgated to SmartView : " + e.NavigationMode.ToString() + " param " + e.Uri.ToString());
        }

        void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                AttachHardwareButtonHandlers();
            }
            else
            {
                RemoveHardwareButtonHandlers();
            }
        }

        /// <summary>
        /// Native Hardware events
        /// </summary>
        void RemoveHardwareButtonHandlers()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                if (page != null)
                {
                    // remove event handler for back key. 
                    page.BackKeyPress -= Page_BackKeyPress;

                }
            }
        }

        /// <summary>
        /// Back key event handler Implementation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_BackKeyPress(object sender, CancelEventArgs e)
        {

            try
            {
                if (!UIUtility.IsPopUp)
                {
                    // TODO : write script for back button
                    string notificationMethod = JS_NOTIFICATION_FROM_NATIVE;
                    string notificationArgument = "" + SmartConstants.NATIVE_EVENT_BACK_PRESSED;
                    string javaScript = NotifyToJavaScript(notificationMethod, notificationArgument);
                    this.ExecuteJavaScript(javaScript);
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while invoking backbutton into view: " + ex.Message);
                e.Cancel = true;
            }


        }




        /// <summary>
        /// Calls JavaScript function with or without arguments as per the
        /// requirement.
        /// </summary>
        /// <param name="javaScriptCallback"></param>
        /// <param name="jsNameToCallArgument"></param>
        /// <returns>string</returns>
        private string NotifyToJavaScript(string javaScriptCallback, string jsNameToCallArgument)
        {
            StringBuilder url = new StringBuilder();
            bool isJSCallback = (javaScriptCallback != null && javaScriptCallback.Length > 0);
            bool isJSArgument = (jsNameToCallArgument != null && jsNameToCallArgument.Length > 0);

            if (isJSCallback)
            {
                url.Append("javascript:");
                url.Append(javaScriptCallback);

                if (jsNameToCallArgument != null && jsNameToCallArgument.Length > 0)
                {
                    jsNameToCallArgument = jsNameToCallArgument.Replace(SmartConstants.SEPARATOR_NEW_LINE, "");
                    jsNameToCallArgument = jsNameToCallArgument.Replace(SmartConstants.HORIZONTAL_TAB, "");
                    // TODO Need to discuss as to how to handle \" in the response
                    // string
                    // and with which character(s),it needs to be replaced
                    jsNameToCallArgument = jsNameToCallArgument.Replace(SmartConstants.ESCAPE_SEQUENCE_BACKSLASH_DOUBLEQUOTES, "");
                    //fix to avoid char by char iteration for replacing "\'" char.
                    if (jsNameToCallArgument.Length > 2)
                    {
                        string jsNametoCallFirstChar = jsNameToCallArgument.Substring(0, 1);
                        string jsNametoCallSubStr = jsNameToCallArgument.Substring(1, jsNameToCallArgument.Length - 2);
                        string jsNametoCallLastChar = jsNameToCallArgument.Substring(jsNameToCallArgument.Length - 1, 1);
                        jsNametoCallSubStr = jsNametoCallSubStr.Replace("\'", "");
                        jsNameToCallArgument = jsNametoCallFirstChar + jsNametoCallSubStr + jsNametoCallLastChar;
                    }

                    url.Append("(");
                    url.Append(jsNameToCallArgument);
                    url.Append(")");
                }
                else if (!isJSArgument && !javaScriptCallback.Contains("(") && !javaScriptCallback.Contains(")"))
                {
                    url.Append("()");
                }

                return url.ToString();
            }

            return url.ToString();
        }

        /// <summary>
        /// Invoke javascript in WebBrowser.
        /// </summary>
        /// <param name="javaScript"></param>
        public void ExecuteJavaScript(string javaScript)
        {
            if (webView != null)
            {
                try
                {
                    string[] javaScriptString = { javaScript };
                    webView.InvokeScript("eval", javaScriptString);
                }
                catch (Exception exception)
                {
                    NotifyToJavaScript(JS_HANDLE_NATIVE_EXCEPTION, exception.Message.ToString());
                }
            }
        }


        /// <summary>
        /// Receive notification from javascript.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            string url = e.Value;

            if (url.Contains("imrlog"))
            {
                string message = url.Replace("imrlog://", "");
                Debug.WriteLine(message + " " + DateTime.Now);
            }
            else if (url.Contains("imr"))
            {
                try
                {
                    string smartMessage = url.Replace("imr://", "");


                    JObject messageObj = JObject.Parse(smartMessage);
                    JToken tempToken;
                    // First check if the message type is SmartEvent or Notifier event
                    if (messageObj.TryGetValue(CommMessageConstants.MMI_MESSAGE_PROP_TRANSACTION_REQUEST, out tempToken))
                    {
                        // Means that it is a SmartEvent

                        smartConnector.ProcessSmartEvent(smartMessage);
                    }
                    else if (messageObj.TryGetValue(NotifierMessageConstants.NOTIFIER_PROP_TRANSACTION_REQUEST, out tempToken))
                    {
                        // Means that it is a Notifier Event
                        ProcessNotifierEvent(smartMessage);

                    }
                    else
                    {
                        // Do nothing
                    }

                }
                catch (MobiletException exception)
                {
                    string javaScript = NotifyToJavaScript(JS_HANDLE_NATIVE_EXCEPTION, exception.Message.ToString());
                    this.ExecuteJavaScript(javaScript);
                }
            }

        }


        private void ProcessNotifierEvent(string notifierMessage)
        {
            // Means that it is a Notifier Event
            NotifierEventProcessor notifierEventProcessor = new NotifierEventProcessor(this);
            NotifierEvent notifierEvent = new NotifierEvent(notifierMessage);
            notifierEventProcessor.ProcessNotifierRegistrationReq(notifierEvent);
        }


        /// <summary>
        /// Sends success notification to Java Script in case of SmartEvent
        /// processing with success 
        /// </summary>
        /// <param name="smartEvent"></param>
        public void OnFinishProcessingWithOptions(appez.model.SmartEvent smartEvent)
        {
            string jsCallback = smartEvent.GetJavaScriptNameToCall();
            string jsCallbackArg = smartEvent.GetJavaScriptNameToCallArg();
            string javaScript = NotifyToJavaScript(jsCallback, "" + jsCallbackArg);
            this.ExecuteJavaScript(javaScript);
        }

        /// <summary>
        /// Sends error notification to Java Script in case of SmartEvent processing
        /// with error
        /// </summary>
        /// <param name="smartEvent"></param>
        public void OnFinishProcessingWithError(appez.model.SmartEvent smartEvent)
        {
            string jsCallback = smartEvent.GetJavaScriptNameToCall();
            string jsCallbackArg = smartEvent.GetJavaScriptNameToCallArg();
            string javaScript = NotifyToJavaScript(jsCallback, "" + jsCallbackArg);
            this.ExecuteJavaScript(javaScript);
        }

        public void ShutDown()
        {
            if (smartConnector != null)
            {
                smartConnector.ShutDown();
            }
            smartConnector = null;
            webView = null;
        }

        /// <summary>
        /// Handles web breowser hide/show.
        /// </summary>
        /// <param name="smartEvent"></param>
        public void OnReceiveContextNotification(SmartEvent smartEvent)
        {
            int notificationCode = smartEvent.GetServiceOperationId();

            switch (notificationCode)
            {
                case CoEvents.CONTEXT_WEBVIEW_HIDE:
                    this.hideSplash = false;
                    this.ToggleSplashScreen();
                    break;

                case CoEvents.CONTEXT_WEBVIEW_SHOW:
                    this.hideSplash = true;
                    this.ToggleSplashScreen();
                    break;

                default:
                    break;
            }
            try
            {
                JObject contextResponse = new JObject();
                contextResponse.Add("response", "OK");
                SmartEventResponse smEventResponse = new SmartEventResponse();
                smEventResponse.IsOperationComplete = true;
                smEventResponse.ServiceResponse = contextResponse.ToString();
                smEventResponse.ExceptionType = 0;
                smEventResponse.ExceptionMessage = null;
                smartEvent.SmartEventResponse = smEventResponse;

                OnFinishProcessingWithOptions(smartEvent);
            }
            catch (JsonException)
            {
                OnFinishProcessingWithError(smartEvent);
            }


        }

        /// <summary>
        /// Initialise Smart connector here with reference of outer activity to
        /// receive SmartAppListener notifications
        /// </summary>
        /// <param name="appListener">SmartAppListener</param>
        public void RegisterAppListener(SmartAppListener appListener)
        {
            SmartAppListener smartAppListener = null;
            if (appListener != null && this.smartConnector != null)
            {
                if (appListener is SmartAppListener)
                {
                    smartAppListener = (SmartAppListener)appListener;
                }
                else
                {
                    throw new MobiletException(ExceptionTypes.SMART_APP_LISTENER_NOT_FOUND_EXCEPTION);
                }
                this.smartConnector.RegisterAppListener(smartAppListener);
            }

        }

        /// <summary>
        /// Create appbar from menu information.
        /// </summary>
        /// <param name="menuItem">Menu item on current page.</param>
        public void ShowOptionMenu(string menuItem)
        {
            UIUtility uiUtility = new UIUtility(this);
            uiUtility.ShowMenu(menuItem);
        }

        /// <summary>
        /// Notify to javascript on menu item click.
        /// </summary>
        /// <param name="actionName">menu item</param>
        public void SendActionInfo(string actionName)
        {
            string notificationMethod = JS_NOTIFICATION_FROM_NATIVE;
            string notificationArgument = actionName;
            string notifyJavaScript = this.NotifyToJavaScript(notificationMethod, notificationArgument);
            this.ExecuteJavaScript(notifyJavaScript);
        }
        /// <summary>
        /// Navigate smartview to url
        /// </summary>
        /// <param name="url"></param>
        public void NavigateToUrl(Uri url)
        {
            this.webView.Navigate(url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void OnReceiveNotifierEventSuccess(NotifierEvent notifierEvent)
        {

            if (appConfigInformation != null)
            {

                string jsListener = GetNotifierListener(notifierEvent.Type);
                string jsListenerArg = notifierEvent.GetJavaScriptNameToCallArg();

                string javaScript = NotifyToJavaScript(jsListener, jsListenerArg);
                Dispatcher.BeginInvoke(() =>
                {
                    this.ExecuteJavaScript(javaScript);
                });
            }

        }


        public void OnReceiveNotifierEventError(NotifierEvent notifierEvent)
        {
            if (appConfigInformation != null)
            {
                string jsListener = GetNotifierListener(notifierEvent.Type);
                string jsListenerArg = notifierEvent.GetJavaScriptNameToCallArg();

                string javaScript = NotifyToJavaScript(jsListener, jsListenerArg);
                Dispatcher.BeginInvoke(() =>
                {
                    this.ExecuteJavaScript(javaScript);
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notifierType"></param>
        /// <returns></returns>
        private String GetNotifierListener(int notifierType)
        {
            string listener = null;
            try
            {
                switch (notifierType)
                {
                    case NotifierConstants.PUSH_MESSAGE_NOTIFIER:
                        listener = appConfigInformation.GetValue(SmartConstants.APPEZ_CONF_PROP_PUSH_NOTIFIER_LISTENER).ToString();
                        break;
                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            return listener;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void OnReceiveNotifierRegistrationEventSuccess(NotifierEvent notifierEvent)
        {
            string jsCallback = "appez.mmi.getMobiletManager().processNotifierResponse";
            string jsCallbackArg = notifierEvent.GetJavaScriptNameToCallArg();
            string javaScript = NotifyToJavaScript(jsCallback, jsCallbackArg);
            Dispatcher.BeginInvoke(() =>
            {
                this.ExecuteJavaScript(javaScript);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notifierEvent"></param>
        public void OnReceiveNotifierRegistrationEventError(NotifierEvent notifierEvent)
        {
            string jsCallback = "appez.mmi.getMobiletManager().processNotifierResponse";
            string jsCallbackArg = notifierEvent.GetJavaScriptNameToCallArg();
            string javaScript = NotifyToJavaScript(jsCallback, jsCallbackArg);
            Dispatcher.BeginInvoke(() =>
            {
                this.ExecuteJavaScript(javaScript);
            });
        }
    }
}

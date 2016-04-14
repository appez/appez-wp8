using appez.appstartup;
using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace appez.utility
{
    /// <summary>
    /// Responsible for creating the dialogs for the UI service
    /// </summary>
    public class UIUtility
    {
        #region variables
        private static ApplicationBar appBar;
        private static ActivityIndicator progress;
        private SmartNotificationListener smartNotificationListener = null;
        private DialogListener dialogListener = null;
        private static Popup popup = null;
        private static List<MenuDetails> menuDetail = null;
        private String dialogTitle = null;
        private String dialogText = null;
        #endregion

        public static bool IsPopUp { get; set; }

        public UIUtility()
        {
        }
        public UIUtility(DialogListener dialogListener)
        {
            this.dialogListener = dialogListener;
        }
        public UIUtility(SmartNotificationListener smartNotificationListener)
        {
            this.smartNotificationListener = smartNotificationListener;
        }
        /// <summary>
        /// Shows appbar according to menu information passed in.
        /// </summary>
        /// <param name="MenuInformationString">Menu information in JSON</param>
        public void ShowMenu(string MenuInformationString)
        {
            // Get all menu information.
            List<MenuInfoBean> source = AppStartupManager.GetMenuInfoCollection();
            int sourceCount = source.Count;

            // If no information present return.
            if ((source == null) && (sourceCount < 0))
            {
                return;
            }

            // Parse menu information string to MenuDetail collection. 
            menuDetail = ParseMenuDetail(MenuInformationString);

            int menuDetailItemCount = menuDetail.Count;
            if (menuDetailItemCount > 0)
            {
                // Check for application bar exist.
                if (appBar == null)
                {
                    // Create application bar.
                    appBar = CreateAppBar();
                }
                else
                {
                    // If application bar already exist, clear menuItem, ApplicationBarIconButton and hide appbar.
                    appBar.MenuItems.Clear();
                    appBar.Buttons.Clear();
                    appBar.IsVisible = true;
                }
                for (int i = 0; i < menuDetailItemCount; i++)
                {
                    for (int j = 0; j < sourceCount; j++)
                    {
                        if (menuDetail[i].MenuId.Equals(source.ElementAt<MenuInfoBean>(j).MenuId))
                        {
                            // If menuIcon url exist create icon button or create menuItem.
                            if (source.ElementAt<MenuInfoBean>(j).MenuIcon != "")
                            {
                                ApplicationBarIconButton button = new ApplicationBarIconButton();
                                button.IconUri = new Uri(source.ElementAt<MenuInfoBean>(j).MenuIcon, UriKind.Relative);
                                button.Text = source.ElementAt<MenuInfoBean>(j).MenuLabel;
                                appBar.Buttons.Add(button);
                                button.Click += new EventHandler(AppBarButton_Click);
                                break;
                            }
                            else
                            {
                                ApplicationBarMenuItem menuItem = new ApplicationBarMenuItem();
                                menuItem.Text = source.ElementAt<MenuInfoBean>(j).MenuLabel;
                                appBar.MenuItems.Add(menuItem);
                                menuItem.Click += new EventHandler(AppBarmenuItem_Click);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                if (appBar != null)
                {
                    // If application bar already exist, clear menuItem, ApplicationBarIconButton and hide appbar.
                    appBar.MenuItems.Clear();
                    appBar.Buttons.Clear();
                    appBar.IsVisible = true;
                }
            }
        }

        /// <summary>
        /// Handles application bar menu item click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarmenuItem_Click(object sender, EventArgs e)
        {

            string str = ((ApplicationBarMenuItem)sender).Text;
            if (smartNotificationListener != null)
            {
                smartNotificationListener.SendActionInfo("'" + str + "'");
            }
        }

        /// <summary>
        /// Handles application bar icon button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AppBarButton_Click(object sender, EventArgs e)
        {

            string str = ((ApplicationBarIconButton)sender).Text;
            if (smartNotificationListener != null)
            {
                smartNotificationListener.SendActionInfo("'" + str + "'");
            }
        }
        public void ShowIndicator()
        {
            if (progress == null)
            {
                progress = new ActivityIndicator();
            }

            progress.ProgressType = ProgressTypes.WaitCursor;
            progress.ShowLabel = true;
            progress.Show();
        }
        public void HideIndicator()
        {
            if (progress != null)
            {
                progress.Hide();
            }
        }

        public void SetProgressBarMessage(String msgStr)
        {
            if (progress != null)
            {
                progress.defaultText = msgStr;
            }

        }
        public void ShowProgressBarWithMessage(String msgStr)
        {
            if (progress == null)
            {
                progress = new ActivityIndicator();
            }

            progress.ProgressType = ProgressTypes.WaitCursor;
            progress.ShowLabel = true;
            progress.defaultText = msgStr;
            progress.Show();

        }
        public void HideActivityView()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (progress != null)
                    {
                        progress.Hide();
                    }

                });
        }
        public void ShowChildPopup(UIElement element)
        {
            popup = new Popup();
            popup.Child = element;
            popup.IsOpen = true;
            IsPopUp = true;
        }

        public void HideChildPopup()
        {
            if (popup != null)
            {
                popup.IsOpen = false;
                IsPopUp = false;
            }
        }
        /// <summary>
        /// Parse menu information string to list of MenuDetails.
        /// </summary>
        /// <param name="MenuInformationString"></param>
        /// <returns></returns>
        private List<MenuDetails> ParseMenuDetail(string MenuInformationString)
        {
            try
            {
                MenuInformation menuInfo = JsonConvert.DeserializeObject<MenuInformation>(MenuInformationString);
                Dictionary<string, object> menuDetailsJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(menuInfo.MenuId.ToString());
                JArray arrMenuDetail = (JArray)menuDetailsJson["menuDetails"];

                List<MenuDetails> menuInfoList = new List<MenuDetails>();

                foreach (object item in arrMenuDetail)
                {
                    MenuDetails menuDetails = JsonConvert.DeserializeObject<MenuDetails>(item.ToString());
                    menuInfoList.Add(menuDetails);
                }

                return menuInfoList;
            }
            catch (JsonReaderException jsonReaderException)
            {
                System.Diagnostics.Debug.WriteLine("Exception while parsing menu information json into list: " + jsonReaderException.Message);
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            catch (KeyNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("Exception while parsing menu information json into list: menuDetails key not found.");
                throw new MobiletException(ExceptionTypes.ERROR_RETRIEVE_DATA_PERSISTENCE);
            }
        }

        /// <summary>
        /// Create application bar for current page.
        /// </summary>
        /// <returns></returns>
        private ApplicationBar CreateAppBar()
        {
            PhoneApplicationPage page = (Application.Current.RootVisual as ContentControl).Content as PhoneApplicationPage;
            page.ApplicationBar = new ApplicationBar();
            appBar = (ApplicationBar)page.ApplicationBar;
            appBar.Mode = 0;
            appBar.Opacity = 1.0;
            appBar.IsVisible = true;
            appBar.IsMenuEnabled = true;
            return appBar;
        }

        /// <summary>
        /// Crete Date picker control
        /// </summary>
        internal void CreateDatePicker()
        {
            DatePicker datePicker = new DatePicker();
            datePicker.PickerPageUri = new Uri("/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml", UriKind.RelativeOrAbsolute);
            datePicker.OpenPickerPage();
            datePicker.ValueChanged += DatePicker_ValueChanged;

        }

        private void DatePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            string dateTimeStr;
            if (null != e.NewDateTime)
            {
                dateTimeStr = e.NewDateTime.Value.ToString("MM-dd-yyyy");
            }
            else
            {
                dateTimeStr = SmartConstants.USER_SELECTION_CANCEL;
            }
            if (dialogListener != null)
            {

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    dialogListener.ProcessUsersSelection("'" + dateTimeStr + "'");
                });

            }
        }
        public void CreateDialog(int id, String message)
        {

            MessageBoxResult messageResult = 0;
            SetMessageDialogTitleText(message);
            switch (id)
            {
                case WebEvents.WEB_SHOW_MESSAGE:
                    {
                        messageResult = MessageBox.Show(dialogText, dialogTitle, MessageBoxButton.OK);
                    }
                    break;
                case WebEvents.WEB_SHOW_MESSAGE_YESNO:
                    {
                        messageResult = MessageBox.Show(dialogText, dialogTitle, MessageBoxButton.OKCancel);
                    }
                    break;


            }
            int intMessageResult = ProcessMessageBoxResult(messageResult);

            if (dialogListener != null)
            {
                dialogListener.ProcessUsersSelection(intMessageResult.ToString());
            }

        }
        /// <summary>
        /// Convert Message box result to int value.
        /// </summary>
        /// <param name="messageResult"></param>
        /// <returns></returns>
        private int ProcessMessageBoxResult(MessageBoxResult messageResult)
        {
            switch (messageResult)
            {
                case MessageBoxResult.OK:
                case MessageBoxResult.Yes:
                    return 0;
                case MessageBoxResult.Cancel:
                case MessageBoxResult.No:
                    return 1;
                default:
                    return -1;

            }
        }

        private void SetMessageDialogTitleText(String message)
        {
            // Initially default values are set for dialog text and titles
            dialogTitle = "Information";
            dialogText = message;

            if (message.Contains(SmartConstants.MESSAGE_DIALOG_TITLE_TEXT_SEPARATOR))
            {
                String[] dialogAttributes = message.Split(new string[] { SmartConstants.MESSAGE_DIALOG_TITLE_TEXT_SEPARATOR }, StringSplitOptions.None);
                if (dialogAttributes.Length == 2)
                {
                    dialogTitle = dialogAttributes[0];
                    dialogText = dialogAttributes[1];
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using appez.constants;
using System.Text;
using appez.listeners;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace appez.utility.uicontrols.contentpicker
{
    public partial class SmartMessagePickerView : PhoneApplicationPage
    {
        string typeOfList = null;
        DialogListener smartPickerListener = null;
        UIUtility uiUtility = null;
        /// <summary>
        /// Initialize smart message picker view.
        /// </summary>
        public SmartMessagePickerView()
        {
            InitializeComponent();
        }

        public SmartMessagePickerView(string messageList, string type, DialogListener listener)
        {
            InitializeComponent();
            typeOfList = type;
            smartPickerListener = listener;
            uiUtility = new UIUtility();
            // Get data from message string and fill into list.
            List<Data> list = new List<Data>();
            string[] messages = ProcessSelectionListItemInfo(messageList);
            int i = 0;
            foreach (string itemName in messages)
            {
                Data dataItem = new Data() { Id = i++, Name = itemName, Type = type, IsCheck = false };
                list.Add(dataItem);
            }
            this.listBox.ItemsSource = list;
            // If list is normal then process user selection on click.
            if (typeOfList.Equals("Normal"))
            {
                StackPanelOkCancel.Visibility = System.Windows.Visibility.Collapsed;
                listBox.SelectionChanged += ListBox_SelectionChanged;
            }

        }
        /// <summary>
        /// Handles listbox selection change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // check for valid selection change.
            if (this.listBox.SelectedIndex > -1)
            {
                uiUtility.HideChildPopup();

                if (smartPickerListener != null)
                {
                    smartPickerListener.ProcessUsersSelection(this.listBox.SelectedIndex.ToString());
                }
                // remove listbox selection change.
                listBox.SelectionChanged -= ListBox_SelectionChanged;
            }
        }
        /// <summary>
        /// Handles button OK click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickOK(object sender, System.Windows.RoutedEventArgs e)
        {

            uiUtility.HideChildPopup();
            string selectedIndicesString = "";
            if (typeOfList.Equals("Normal"))
            {
                selectedIndicesString =  this.listBox.SelectedIndex.ToString();
            }
            else if (typeOfList.Equals("Radio"))
            {
                
                List<Data> list = this.listBox.ItemsSource as List<Data>;
                foreach (var data in list)
                {
                    if (data.IsCheck == true)
                    {
                        selectedIndicesString = data.Id.ToString();
                    }
                }

            }
            else if (typeOfList.Equals("Checkbox"))
            {

                List<Data> list = this.listBox.ItemsSource as List<Data>;
                JArray selectedIndicesArray = new JArray();
                try
                {
                    foreach (var data in list)
                    {
                        if (data.IsCheck == true)
                        {
                            JObject selectedIndexObj = new JObject();
                            selectedIndexObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_USER_SELECTED_INDEX, data.Id.ToString());
                            selectedIndicesArray.Add(selectedIndexObj);

                        }
                    }
                    selectedIndicesString = selectedIndicesArray.ToString();

                }
                catch (JsonException)
                {
                    selectedIndicesString = "";
                }
            }
            if (smartPickerListener != null)
            {
                smartPickerListener.ProcessUsersSelection(selectedIndicesString);
            }
        }

        private void OnClickCancel(object sender, System.Windows.RoutedEventArgs e)
        {
            uiUtility.HideChildPopup();
            if (smartPickerListener != null)
            {
                smartPickerListener.ProcessUsersSelection(SmartConstants.USER_SELECTION_CANCEL);
            }
        }
        private string[] ProcessSelectionListItemInfo(string listInfo)
        {
            List<string> listElements = new List<string>();
           
            try
            {
                JArray listInfoObj = JArray.Parse(listInfo);
                if (listInfoObj != null && listInfoObj.Count > 0)
                {
                 
                    foreach (var item in listInfoObj.Children())
                    {
                        JObject keyValuePairObj = item.ToObject<JObject>();
                        listElements.Add(keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_ITEM).ToString());
                        
                    }
                   
                }
                
            }
            catch (JsonException)
            {
                listElements = null;
            }

            return listElements.ToArray();
        }
    }

    public class PickerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }

        public DataTemplate Checkbox { get; set; }

        public DataTemplate Radio { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Data dataItem = item as Data;
            if (dataItem != null)
            {
                if (dataItem.Type == "Normal")
                {
                    return Normal;
                }
                else if (dataItem.Type == "Radio")
                {
                    return Radio;
                }
                else
                {
                    return Checkbox;
                }
            }

            return base.SelectTemplate(item, container);
        }
        
    }
    /// <summary>
    /// Data model to be filled in list.
    /// </summary>
    public class Data
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsCheck { get; set; }

        public string Type { get; set; }
    }

}
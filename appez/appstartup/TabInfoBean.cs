using System;

namespace appez.appstartup
{
    /// <summary>
    /// Model for holding all the required information fields,
    /// corresponding to a single application tab.(Applicable to tab based
    /// application only)
    /// </summary>
    public class TabInfoBean
    {
        
        /// <summary>
        /// Indicates the label of the tab item. This label is the one that appears
        /// in the tab bar of the application
        /// </summary>
        public string TabLabel { set; get; }
        /// <summary>
        /// Indicates the icon of the tab item.
        /// </summary>
        public string TabIcon { set; get; }
        /// <summary>
        /// Indicates a unique ID assigned to the tab item. This is specified by the
        /// application developer for its reference.
        /// </summary>
        public string TabId { set; get; }
        /// <summary>
        /// Indicates the page URL which is associated with this tab. On selecting
        /// this tab, the page specified gets loaded.
        /// </summary>
        public string TabContentUrl { set; get; }
    }
}

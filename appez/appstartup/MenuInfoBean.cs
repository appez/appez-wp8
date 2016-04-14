using System;

namespace appez.appstartup
{
    /// <summary>
    /// Model for holding all the required information fields,
    /// corresponding to a single application menu
    /// </summary>
    public class MenuInfoBean
    {
        /// <summary>
        /// Indicates the label of the menu item. This label is the one that appears
        /// in the application.
        /// </summary>
        public string MenuLabel{set ; get ;}
        /// <summary>
        /// Indicates the icon of the menu item. This label is the one that appears
        /// in the application.
        /// </summary>
        public string MenuIcon { set; get; }
        /// <summary>
        /// Indicates a unique ID assigned to the menu item. This is specified by the
        /// application developer for its reference. When any menu item is selected,
        /// this menu ID is passed to the web layer.
        /// </summary>
        public string MenuId { set; get; }

        
    }
}

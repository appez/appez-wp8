using appez.exceptions;
using appez.constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace appez.appstartup
{
    /// <summary>
    /// AppStartupManager: Processes the startup information receives from the web
    /// layer at the start of the application. Typically this contains information
    /// regarding the styling of the native action bar, styling information of the
    /// action bar tabs(in case of tab based application) along with the menu
    /// information of all the application menus
    /// </summary>
    public class AppStartupManager
    {
        #region variables
        private static List<MenuInfoBean> menuInfoBeanCollection = new List<MenuInfoBean>();
        private static List<TabInfoBean> tabInfoBeanCollection = new List<TabInfoBean>();
        #endregion

        public AppStartupManager()
        {

        }

        /// <summary>
        /// User application calls this method when the startup information is
        /// received from the web layer. Invoked in the 'AppInitActivity' of the
        /// client application.
        /// </summary>
        /// <param name="appStartupInfo"></param>
        public static void ProcessAppStartupInfo(string appStartupInfo)
        {
            try
            {
                if (appStartupInfo != null)
                {

                    Dictionary<string, Object> json = JsonConvert.DeserializeObject<Dictionary<string, Object>>(appStartupInfo);

                    if (json.ContainsKey(SmartConstants.APP_STARTUP_INFO_NODE_MENUS))
                    {

                        AppStartupManager.ProcessMenuCreationInfo((JArray)json[SmartConstants.APP_STARTUP_INFO_NODE_MENUS]);
                    }
                    if (json.ContainsKey(SmartConstants.APP_STARTUP_INFO_NODE_TABS))
                    {

                        AppStartupManager.ProcessTabCreationInfo((JArray)json[SmartConstants.APP_STARTUP_INFO_NODE_TABS]);
                    }

                }
                else
                {
                    throw new MobiletException();
                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }

       
        /// <summary>
        /// Processes information regarding application menus and puts them in
	    /// relevant collection
        /// </summary>
        /// <param name="menuCreationInfo">JArray that contains the collection of all the menu
	    /// options available in the application</param>
        private static void ProcessMenuCreationInfo(JArray menuCreationInfo)
        {
            JArray menuNodes = menuCreationInfo;
            try
            {
                // looping through menu information nodes
                foreach (dynamic menuToAdd in menuCreationInfo)
                {
                    MenuInfoBean menuInfoBean = new MenuInfoBean();

                    menuInfoBean.MenuLabel = (string)menuToAdd[SmartConstants.MENUS_CREATION_PROPERTY_LABEL];
                    menuInfoBean.MenuIcon = (string)menuToAdd[SmartConstants.MENUS_CREATION_PROPERTY_ICON];
                    menuInfoBean.MenuId = (string)menuToAdd[SmartConstants.MENUS_CREATION_PROPERTY_ID];

                    menuInfoBeanCollection.Add(menuInfoBean);
                }


            }
            catch (InvalidCastException)
            {
                throw new MobiletException();
            }
            catch (KeyNotFoundException)
            {
                throw new MobiletException();
            }
            catch (Exception)
            {
                throw new MobiletException();
            }
        }

        public static List<MenuInfoBean> GetMenuInfoCollection()
        {
            return menuInfoBeanCollection;
        }

        /// <summary>
        /// Processes information regarding application tabs and puts them in
	    /// relevant collection. Applicable for tab based application only.
        /// </summary>
        /// <param name="tabCreationInfo">JSONArray that contains the collection of all the tabs to be
	    /// constructed in the application</param>
        private static void ProcessTabCreationInfo(JArray tabCreationInfo)
        {
            try
            {
                // looping through menu information nodes
                foreach (dynamic tabToAdd in tabCreationInfo)
                {
                    TabInfoBean tabInfoBean = new TabInfoBean();

                    tabInfoBean.TabLabel = (string)tabToAdd[SmartConstants.TABS_CREATION_PROPERTY_LABEL];
                    tabInfoBean.TabIcon = (string)tabToAdd[SmartConstants.TABS_CREATION_PROPERTY_ICON];
                    tabInfoBean.TabId = (string)tabToAdd[SmartConstants.TABS_CREATION_PROPERTY_ID];
                    tabInfoBean.TabContentUrl = (string)tabToAdd[SmartConstants.TABS_CREATION_PROPERTY_CONTENT_URL];

                    tabInfoBeanCollection.Add(tabInfoBean);
                }

            }
            catch (InvalidCastException)
            {
                throw new MobiletException();
            }
            catch (KeyNotFoundException)
            {
                throw new MobiletException();
            }
            catch (Exception)
            {
                throw new MobiletException();
            }
        }

        public static List<TabInfoBean> GetTabInfoCollection()
        {
            return tabInfoBeanCollection;
        }

    }
}

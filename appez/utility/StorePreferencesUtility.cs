using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;

namespace appez.utility
{
    /// <summary>
    /// This class will store shared preferences values in key value format.
    /// </summary>
    public class StorePreferencesUtility
    {
        #region variables
        private String preferenceName = "appez";
        IsolatedStorageSettings isolatedStore;
        Dictionary<String, String> currentPreferences = null;
        #endregion
        public StorePreferencesUtility()
        {
            // Get the settings for this application.
            isolatedStore = IsolatedStorageSettings.ApplicationSettings;

        }

        /// <summary>
        /// Set preference name.
        /// </summary>
        /// <param name="name">Preference name</param>
        public void SetPreferenceName(String name)
        {

            if (name != null && name.Length > 0)
            {
                this.preferenceName = name;
            }
            if (isolatedStore != null && isolatedStore.Contains(this.preferenceName))
            {
                currentPreferences = (Dictionary<String, String>)isolatedStore[this.preferenceName];
            }
            else
            {
                this.currentPreferences = new Dictionary<string, string>();
                isolatedStore.Add(this.preferenceName, currentPreferences);
            }
            isolatedStore.Save();


        }

        /// <summary>
        /// Store preference values
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>status</returns>
        public bool SetPreference(String key, String value)
        {

            bool valueChanged = false;

            // If the key exists
            if (this.currentPreferences.ContainsKey(key))
            {
                this.currentPreferences[key] = value;
                // Store the new value

                valueChanged = true;
            }
            // Otherwise create the key.
            else
            {
                this.currentPreferences.Add(key, value);
                valueChanged = true;
            }

            isolatedStore[this.preferenceName] = this.currentPreferences;
            isolatedStore.Save();

            return valueChanged;
        }

        /// <summary>
        /// Retrieve all the entries from the Preferences
        /// </summary>
        /// <returns>Collection of key value stored in preference</returns>
        public Dictionary<String, String> GetAllFromPreference()
        {
            if (currentPreferences != null)
            {
                return currentPreferences;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get value from preference
        /// </summary>
        /// <param name="key">key whose value is needed</param>
        /// <returns>value</returns>
        public String GetPreference(String key)
        {
            String value = null;

            if (this.currentPreferences != null && this.currentPreferences.ContainsKey(key))
            {
                value = (String)this.currentPreferences[key];
                isolatedStore.Save();
            }
            return value;
        }

        /// <summary>
        /// Delete entry from preference.
        /// </summary>
        /// <param name="key">key to be remove</param>
        /// <returns>operation status.</returns>
        public bool RemoveFromPreference(String key)
        {
            bool valueChanged = false;
            if (this.currentPreferences != null && this.currentPreferences.ContainsKey(key))
            {
                valueChanged = currentPreferences.Remove(key);
            }
            else
            {
                valueChanged = false;
            }

            isolatedStore[this.preferenceName] = this.currentPreferences;
            isolatedStore.Save();

            return valueChanged;
        }


    }
}

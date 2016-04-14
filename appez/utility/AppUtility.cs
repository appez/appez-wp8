using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;

namespace appez.utility
{
    /// <summary>
    /// Generic utility class that contain various method
    /// used in application.
    /// </summary>
    public static class AppUtility
    {
        /// <summary>
        /// Utility method to delete folder
        /// </summary>
        /// <param name="oldAssetLocation">Folder location</param>
        /// <returns>Result</returns>
        public static bool DeleteFolder(string oldAssetLocation)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string folderName = Path.GetDirectoryName(oldAssetLocation);
                DeleteFolder(isoStore,folderName);
                if (isoStore.FileExists(oldAssetLocation))
                {
                    return false;
                }
                else
                { return true; }
            }

        }
        /// <summary>
        /// Deletes the folder.
        /// </summary>
        /// The isolated storage folder.
        /// The path to delete.
        private static void DeleteFolder(this IsolatedStorageFile iso, string path)
        {
            if (!iso.DirectoryExists(path))
                return;
            // Get the subfolders that reside under path
            var folders = iso.GetDirectoryNames(Path.Combine(path, "*.*"));

            // Iterate through the subfolders and check for further subfolders           
            foreach (var folder in folders)
            {
                string folderName = path + "/" + folder;
                iso.DeleteFolder(folderName);
            }
            // Delete all files at the root level in that folder.
            foreach (var file in iso.GetFileNames(Path.Combine(path, "*.*")))
            {
                iso.DeleteFile(Path.Combine(path, file));
            }
            // Finally delete the path
            iso.DeleteDirectory(path);
        }
        /// <summary>
        /// Utility method to delete file
        /// </summary>
        /// <param name="oldFileLocation">file location</param>
        /// <returns></returns>
        public static bool DeleteFile(string oldFileLocation)
        {

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoStore.FileExists(oldFileLocation))
                {
                    isoStore.DeleteFile(oldFileLocation);
                }
                if (isoStore.FileExists(oldFileLocation))
                {
                    return false;
                }
                else
                { return true; }
            }
        }
        /// <summary>
        /// Utility method to get the current app version.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationVersion()
        {
            Assembly assembly = Application.Current.GetType().Assembly;
            string version = assembly.GetName().Version.ToString();
            return version;
        }
        /// <summary>
        /// Utility method to get the current app name.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationName()
        {
            Assembly assembly = Application.Current.GetType().Assembly;
            string name = assembly.GetName().Name;
            return name;
        }

        /// <summary>
        /// Utility method to get the current assembly name.
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyName()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetName().Name;
            return name;
        }

        /// <summary>
        /// Utility method to get value of key defined in AppResources.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringForId(string key)
        {
            Assembly currentAssembly = Application.Current.GetType().Assembly;
            string appName = currentAssembly.GetName().Name.ToString();
            var resManager = new ResourceManager(appName + ".Resources.AppResources", currentAssembly);
            return resManager.GetString(key);
        }

        /// <summary>
        /// Utility method to copy file from one location to another.
        /// </summary>
        /// <param name="assetSourceLocation"></param>
        /// <param name="assetDestLocation"></param>
        public static void CopyAsset(string assetSourceLocation, string assetDestLocation)
        { 
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var inputSream = Application.GetResourceStream(new Uri(assetSourceLocation, UriKind.Relative)).Stream)
                {
                    string parentDir = Path.GetDirectoryName(assetDestLocation);
                    if (!isoStore.DirectoryExists(parentDir))
                    {
                        isoStore.CreateDirectory(parentDir);
                    }
                       using(IsolatedStorageFileStream outStream = isoStore.CreateFile(assetDestLocation))
                       {
                            inputSream.Seek(0, SeekOrigin.Begin);
                            inputSream.CopyTo(outStream);
                       }
                }
            }
        }
        /// <summary>
        /// Utility method to copy folder from one location to another.
        /// </summary>
        /// <param name="assetSourceLocation"></param>
        /// <param name="assetDestLocation"></param>
        /// <returns></returns>
        public static async Task CopyAssetFolder(string assetSourceLocation, string assetDestLocation)
        {
            StorageFolder sf = await StorageFolder.GetFolderFromPathAsync(assetSourceLocation);
            string folder = Path.GetFileName(assetSourceLocation);
            string destFolder = Path.Combine(assetDestLocation);
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isoStore.DirectoryExists(destFolder))
                {
                    isoStore.CreateDirectory(destFolder);
                }
                IReadOnlyList<IStorageItem> folderitems = await sf.GetItemsAsync();
                foreach (var item in folderitems)
                {
                    if (item.IsOfType(StorageItemTypes.File))
                    {
                        CopyAsset(Path.GetFullPath(item.Path), Path.Combine(destFolder, item.Name));
                    }

                    Debug.WriteLine("item name: " + item.Name + " item type: " + item.GetType().ToString());
                    if (item.IsOfType(StorageItemTypes.Folder))
                    {
                        await CopyAssetFolder(Path.GetFullPath(item.Path), Path.Combine(destFolder, item.Name));
                    }
                }
            }
        }
        /// <summary>
        /// Utility method for getting app config file property.
        /// </summary>
        /// <param name="configFileLocation"></param>
        /// <returns></returns>
        public static JObject GetAppConfigFileProps(string configFileLocation)
        {
            JObject configFileProps = null;
            string fileContents = null;
            string filePath = "Assets/" + configFileLocation;
            var resourcePath = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));
            fileContents = new StreamReader(resourcePath.Stream).ReadToEnd();

            configFileProps = new JObject();
            String[] allProps = fileContents.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (allProps != null && allProps.Length > 0)
            {
                int configPropertyCount = allProps.Length;
                for (int currentProp = 0; currentProp < configPropertyCount; currentProp++)
                {
                    // Now split the startup information across the '='
                    // separator to get individual key-value pairs
                    String[] configProperty = allProps[currentProp].Split('=');
                    String propKey = configProperty[0].Trim();
                    String propValue = configProperty[1].Trim();
                    configFileProps.Add(propKey, propValue);
                }
            }

            return configFileProps;
        }


        /// <summary>
        /// Get device IMEI number.
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceImei()
        {
            byte[] deviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return Convert.ToBase64String(deviceID);
        }
    }
}

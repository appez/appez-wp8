using appez.constants;
using appez.exceptions;
using appez.listeners;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace appez.utility
{
    /// <summary>
    /// The purpose of this class is to extract the zip archive files given the name
    /// and location of the files. Primarily used in the extraction of assets for the
    /// soft upgrade
    /// </summary>
    public class UnzipUtility
    {
        #region Variable
        private String zipFileName;
        private String zipFileLocation;
        SmartUnzipListener smartUnzipListener = null;
        private bool isUnzipSuccess = true;
        #endregion
        /// <summary>
        /// Initialize unzip util with required information.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="fileLocation">File location</param>
        /// <param name="unzipListener">UnzipListener</param>
        public UnzipUtility(String fileName, String fileLocation, SmartUnzipListener unzipListener)
        {
            zipFileName = fileName;
            zipFileLocation = fileLocation;
            this.smartUnzipListener = unzipListener;


        }
        /// <summary>
        /// Start unzipping data
        /// </summary>
        public void Execute()
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream stream = isoStore.OpenFile(zipFileName, FileMode.Open, FileAccess.Read))
                    {
                        string zipFileAbsPath = Path.GetFullPath(stream.Name);

                        StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                        string unzipDirectory = Path.Combine(localFolder.Path, this.zipFileLocation);
                        FastZip fz = new FastZip();

                        fz.CreateEmptyDirectories = true;
                        fz.ExtractZip(zipFileAbsPath, unzipDirectory, "");
                    }


                }
                catch (FileNotFoundException fnfe)
                {
                    Debug.WriteLine(fnfe.Message.ToString());
                    isUnzipSuccess = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message.ToString());
                    isUnzipSuccess = false;
                }

            }
            try
            {
                if (isUnzipSuccess)
                {
                    JObject zipOperationCompletionResponse = new JObject();
                    zipOperationCompletionResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_UNARCHIVE_LOCATION, zipFileLocation);
                    String unzipData = zipOperationCompletionResponse.ToString();
                    this.smartUnzipListener.OnUnzipOperationCompleteWithSuccess(unzipData);
                }
                else
                {
                    // There was some problem extracting the ZIP file
                    smartUnzipListener.OnUnzipOperationCompleteWithError(ExceptionTypes.FILE_UNZIP_ERROR_MESSAGE);
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }

        }

    }
}

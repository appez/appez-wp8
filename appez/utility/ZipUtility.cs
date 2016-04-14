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
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace appez.utility
{
    /// <summary>
    /// Responsible for creating the ZIP file from the folder/file
    /// location provided by the user
    /// </summary>
    public class ZipUtility
    {
        #region variables
        List<string> fileList;
        private String outputZipFile;
        private String sourceToArchive;

        private bool isZipSuccess = true;
        private SmartZipListener smartZipListener = null;
        #endregion

        public ZipUtility(String source, String targetArchive, SmartZipListener zipListener)
        {
            fileList = new List<string>();
            sourceToArchive = source;
            outputZipFile = targetArchive;
            smartZipListener = zipListener;
            
        }

        public void Execute()
        {
            try
            {
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isoStore.FileExists(outputZipFile))
                    {
                        isoStore.DeleteFile(outputZipFile);
                    }
                    using (IsolatedStorageFileStream stream = isoStore.CreateFile(outputZipFile))
                    {
                        
                        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                        string zipDirectory = Path.Combine(localFolder.Path, this.sourceToArchive);
                        if (!Path.HasExtension(zipDirectory))
                        {
                            FastZip fz = new FastZip();
                            fz.CreateEmptyDirectories = true;
                            fz.CreateZip(stream, zipDirectory, true, "", "");
                        }
                        else
                        {
                            using (ZipOutputStream zipStream = new ZipOutputStream(stream))
                            {
                                byte[] buffer = new byte[4096];
                                ZipEntry newEntry = new ZipEntry(Path.GetFileName(sourceToArchive));

                                using (IsolatedStorageFileStream fileReader = new IsolatedStorageFileStream(sourceToArchive, FileMode.Open, FileAccess.Read, isoStore))
                                {
                                    newEntry.Size = fileReader.Length;
                                    zipStream.PutNextEntry(newEntry);
                                    int sourceBytes;
                                    do
                                    {
                                        sourceBytes = fileReader.Read(buffer, 0, buffer.Length);
                                        zipStream.Write(buffer, 0, sourceBytes);

                                    } while (sourceBytes > 0);
                                }
                            }

                        }

                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                Debug.WriteLine(fnfe.Message.ToString());
                isZipSuccess = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                isZipSuccess = false;
            }
            try
            {
                if (isZipSuccess)
                {
                    JObject zipOperationCompletionResponse = new JObject();
                    zipOperationCompletionResponse.Add(CommMessageConstants.MMI_RESPONSE_PROP_FILE_UNARCHIVE_LOCATION, outputZipFile);
                    String zipData = zipOperationCompletionResponse.ToString();
                    this.smartZipListener.OnZipOperationCompleteWithSuccess(zipData);
                }
                else
                {
                    // There was some problem extracting the ZIP file
                    smartZipListener.OnZipOperationCompleteWithError(ExceptionTypes.FILE_ZIP_ERROR_MESSAGE);
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
           
        }
        
    }
}

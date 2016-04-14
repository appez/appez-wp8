using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.utility;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace appez.services
{
    /// <summary>
    /// Enables the user to read file/folder at the specified
    /// location in the web assets
    /// </summary>
    public class FileService : SmartService, SmartUnzipListener, SmartZipListener
    {
        #region variables
        private SmartServiceListener smartServiceListener = null;
        private SmartEvent smartEvent = null;
        private FileReadUtility fileReadUtility = null;
        private String tempArchiveFileAbsLocation = null;
        private String tempSourceAbsLocation = null;
        private String zipAssetFolderName = null;

        private bool isArchiveToCreateFile = false;
        #endregion
        /// <summary>
        /// Create instance of file read service.
        /// </summary>
        /// <param name="smartServiceListener">SmartEventListener</param>
        public FileService(SmartServiceListener smartServiceListener)
            : base()
        {

            this.smartServiceListener = smartServiceListener;
        }


        public override void ShutDown()
        {

        }

        public override void PerformAction(model.SmartEvent smartEvent)
        {
            try
            {
                this.fileReadUtility = new FileReadUtility(smartEvent.SmartEventRequest.ServiceRequestData);
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            this.smartEvent = smartEvent;

            switch (smartEvent.GetServiceOperationId())
            {
                case WebEvents.WEB_READ_FILE_CONTENTS:
                    ReadFileContents();
                    break;
                case WebEvents.WEB_READ_FOLDER_CONTENTS:
                    ReadFolderContents();
                    break;
                case WebEvents.WEB_UNZIP_FILE_CONTENTS:
                    ExtractArchiveFile();
                    break;
                case WebEvents.WEB_ZIP_CONTENTS:
                    CreateArchiveFile();
                    break;
            }

        }

        
        
        /// <summary>
        /// Read data from specified file.
        /// </summary>
        private void ReadFileContents()
        {
            String fileContents = null;
            try
            {
                // Read the contents of the file using File reading utility
                fileContents = fileReadUtility.GetFileContents().ToString();
                if (fileContents != null)
                {
                    // Provide the data received from file reading operation
                    OnSuccessFileOperation(fileContents);
                }
                else
                {
                    OnErrorFileOperation(ExceptionTypes.IO_EXCEPTION, null);
                }
            }
            catch (System.IO.IOException ioe)
            {
                OnErrorFileOperation(ExceptionTypes.IO_EXCEPTION, ioe.Message.ToString());
            }
            catch (Exception e)
            {
                OnErrorFileOperation(ExceptionTypes.UNKNOWN_EXCEPTION, e.Message.ToString());
            }
        }
        /// <summary>
        /// Read the contents of the folder whose path has been provided by the user
        /// </summary>
        private async void ReadFolderContents()
        {
            String filesInFolder = null;
            try
            {
                filesInFolder = await fileReadUtility.GetFileContentInFolder();
                // Provide the data received from folder reading operation
                OnSuccessFileOperation(filesInFolder);
            }
            catch (IOException ioe)
            {
                OnErrorFileOperation(ExceptionTypes.IO_EXCEPTION, ioe.Message);
            }
            catch (JsonException je)
            {
                OnErrorFileOperation(ExceptionTypes.JSON_PARSE_EXCEPTION, je.Message);
            }
            catch (Exception e)
            {
                OnErrorFileOperation(ExceptionTypes.UNKNOWN_EXCEPTION, e.Message);
            }
        }


        /// <summary>
        /// Extract zip file.
        /// </summary>
        private void ExtractArchiveFile()
        {
            try
            {

                String folderLocation = null;
                String assetArchiveFileLocation = this.smartEvent.SmartEventRequest.ServiceRequestData.GetValue(CommMessageConstants.MMI_REQUEST_PROP_FILE_TO_READ_NAME).ToString();

                // Since the file cannot be read directly from the assets folder of
                // the application, we need to copy it to either external storage or
                // application sandbox, and then unzip it
                CopyArchiveFileToMemory(assetArchiveFileLocation);

                if (assetArchiveFileLocation.EndsWith(".zip"))
                {

                    folderLocation = Path.Combine(Path.GetDirectoryName(assetArchiveFileLocation), Path.GetFileNameWithoutExtension(assetArchiveFileLocation));

                    Debug.WriteLine("FileService->extractArchiveFile->tempArchiveFileAbsLocation:" + tempArchiveFileAbsLocation + ",folderLocation:" + folderLocation);
                    UnzipUtility unzipUtil = new UnzipUtility(tempArchiveFileAbsLocation, folderLocation, this);
                    unzipUtil.Execute();
                }
                else
                {
                    // If the file provided by the user is not of ZIP type, then
                    // return with error
                    OnErrorFileOperation(ExceptionTypes.FILE_UNZIP_ERROR, ExceptionTypes.FILE_UNZIP_ERROR_MESSAGE + "Incorrect file type.");
                }

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
        }
        /// <summary>
        /// Responsible for the creation of ZIP archive file based on the specified
	    /// file/folder by the user
        /// </summary>
        private async void CreateArchiveFile()
        {
            await CopySourceToMemory();

            String targetArchiveFile = tempSourceAbsLocation + ".zip";

            // Add the logic for creating zip archive here
            ZipUtility zipUtility = new ZipUtility(tempSourceAbsLocation, targetArchiveFile, this);
            zipUtility.Execute();
        }

        private async Task CopySourceToMemory()
        {
            try
            {
                string assetSourceLocation = this.smartEvent.SmartEventRequest.ServiceRequestData.GetValue(CommMessageConstants.MMI_REQUEST_PROP_FILE_TO_READ_NAME).ToString();
                assetSourceLocation = "Assets/" + assetSourceLocation;
                zipAssetFolderName = Path.GetFileName(assetSourceLocation);

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    tempSourceAbsLocation = "appez_zip_temp_" + DateTime.Now.ToString("yyyyMMddHHmmss")+Path.DirectorySeparatorChar + zipAssetFolderName;

                    if (zipAssetFolderName.Contains("."))
                    {
                        // Means that it is a file
                        isArchiveToCreateFile = true;
                        AppUtility.CopyAsset(assetSourceLocation, tempSourceAbsLocation);
                    }
                    else
                    {
                        // Means it is a directory
                        // NOTE: The zip operation will be unable to create Zip archive
                        // for the directories whose name contains '.'(Period) because
                        // they will be treated as files
                        await AppUtility.CopyAssetFolder(assetSourceLocation, tempSourceAbsLocation);
                    }
                }
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }
            catch (Exception e)
            { 
                Debug.WriteLine("Error while creating zip file :"+e.Message.ToString());
                throw new MobiletException(ExceptionTypes.UNKNOWN_EXCEPTION);
            }
        }

        /// <summary>
        /// Copy archive file from application package to isolated storage.
        /// </summary>
        /// <param name="archiveFileLocationInAssets">archive file location</param>
        private void CopyArchiveFileToMemory(String archiveFileLocationInAssets)
        {
            this.tempArchiveFileAbsLocation = "temp.zip";
            try
            {
                string filePath = "Assets/" + archiveFileLocationInAssets;
                var resourcePath = Application.GetResourceStream(new Uri(filePath, UriKind.Relative));
                var data = resourcePath.Stream;
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream stream = isoStore.CreateFile(this.tempArchiveFileAbsLocation);
                    data.Seek(0, SeekOrigin.Begin);
                    data.CopyTo(stream);
                    stream.Close();

                }
                data.Close();
            }
            catch (NullReferenceException nre)
            {
                Debug.WriteLine(nre.Message.ToString());
                throw new MobiletException(ExceptionTypes.FILE_NOT_FOUND_EXCEPTION);
            }
            catch (IsolatedStorageException ise)
            {
                Debug.WriteLine(ise.Message.ToString());
                throw new MobiletException(ExceptionTypes.FILE_READ_EXCEPTION);
            }

        }
        /// <summary>
        /// Specifies action to be taken on successful completion of file read
        /// operation
        /// </summary>
        /// <param name="fileContent">Content of the file that was read</param>
        private void OnSuccessFileOperation(string fileResponseData)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = true;
            smEventResponse.ServiceResponse = fileResponseData;
            smEventResponse.ExceptionType = 0;
            smEventResponse.ExceptionMessage = null;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }
        /// <summary>
        /// Specifies action to be taken on unsuccessful completion of file read
        /// operation
        /// </summary>
        /// <param name="exceptionType">Unique ID of the exception denoting failure in reading file contents</param>
        private void OnErrorFileOperation(int exceptionType, string exceptionMessage)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = false;
            // TODO set the response string here
            smEventResponse.ServiceResponse = null;
            smEventResponse.ExceptionType = exceptionType;
            smEventResponse.ExceptionMessage = exceptionMessage;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(smartEvent);
        }
        /// <summary>
        /// Delete temp zip file.
        /// </summary>
        /// <returns></returns>
        private bool DeleteTempArchiveFile()
        {
            return AppUtility.DeleteFile(tempArchiveFileAbsLocation);
        }
        /// <summary>
        /// Delete file from temporary memory location after the zip operation is completed 
        /// </summary>
        /// <returns></returns>
        private bool DeleteTempFile()
        {
            return AppUtility.DeleteFile(tempSourceAbsLocation );
        }
        /// <summary>
        /// Delete folder from temporary memory location after the zip operation is completed
        /// </summary>
        /// <returns></returns>
        private bool DeleteTempFolder()
        {
            string tempSourceLocation = null;
            if (tempSourceAbsLocation != null)
            {
                tempSourceLocation = tempSourceAbsLocation + Path.DirectorySeparatorChar + zipAssetFolderName;
            }
            else
            {
                tempSourceLocation = null;
            }
            return AppUtility.DeleteFolder(tempSourceLocation);
        }
        /// <summary>
        /// Unzip operation success
        /// </summary>
        /// <param name="opCompData">Unzip file info</param>
        public void OnUnzipOperationCompleteWithSuccess(string opCompData)
        {
            if (DeleteTempArchiveFile())
            {
                OnSuccessFileOperation(opCompData);
            }
        }
        /// <summary>
        /// Unzip operation error
        /// </summary>
        /// <param name="errorMessage">error message</param>
        public void OnUnzipOperationCompleteWithError(string errorMessage)
        {

            DeleteTempArchiveFile();
            OnErrorFileOperation(ExceptionTypes.FILE_UNZIP_ERROR, errorMessage);
        }
        /// <summary>
        /// Listener function that informs the successful completion of Zip operation
        /// </summary>
        /// <param name="opCompData">Contains information regarding the archive file</param>
        public void OnZipOperationCompleteWithSuccess(string opCompData)
        {
            bool isDeleteSuccessful = false;
            if (isArchiveToCreateFile)
            {
                isDeleteSuccessful = DeleteTempFile();
            }
            else
            {
                isDeleteSuccessful = DeleteTempFolder();
            }
            if (isDeleteSuccessful)
            {
                OnSuccessFileOperation(opCompData);
            }
        }
        /// <summary>
        /// Listener function that informs the successful completion of Zip operation
        /// </summary>
        /// <param name="errorMessage">Message describing the cause of failure of archiving/zipping operation</param>
        public void OnZipOperationCompleteWithError(string errorMessage)
        {
            // Here we are not checking if the folder/file has deleted successfully
            // or not
            // because in any case the user has to be notified of the error in
            // zipping the source file/folder
            if (isArchiveToCreateFile)
            {
                DeleteTempFile();
            }
            else
            {
                DeleteTempFolder();
            }
            OnErrorFileOperation(ExceptionTypes.FILE_ZIP_ERROR, errorMessage);
        }
    }
}

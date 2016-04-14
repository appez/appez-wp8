using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.constants
{
    /// <summary>
    ///  Constant class that holds all the properties that are used for communication
    /// between the platform native layer and the web layer. This includes both the
    /// request as well as response parameters
    /// </summary>
    public class CommMessageConstants
    {
        //Request Object JSON properties
        public const string MMI_MESSAGE_PROP_TRANSACTION_ID = "transactionId";
        public const string MMI_MESSAGE_PROP_RESPONSE_EXPECTED = "isResponseExpected";
        public const string MMI_MESSAGE_PROP_TRANSACTION_REQUEST = "transactionRequest";
        public const string MMI_MESSAGE_PROP_REQUEST_OPERATION_ID = "serviceOperationId";
        public const string MMI_MESSAGE_PROP_REQUEST_DATA = "serviceRequestData";
        public const string MMI_MESSAGE_PROP_TRANSACTION_RESPONSE = "transactionResponse";
        public const string MMI_MESSAGE_PROP_TRANSACTION_OP_COMPLETE = "isOperationComplete";
        public const string MMI_MESSAGE_PROP_SERVICE_RESPONSE = "serviceResponse";
        public const string MMI_MESSAGE_PROP_RESPONSE_EX_TYPE = "exceptionType";
        public const string MMI_MESSAGE_PROP_RESPONSE_EX_MESSAGE = "exceptionMessage";

        //Service Request object properties
        public const string MMI_REQUEST_PROP_SERVICE_SHUTDOWN = "serviceShutdown";
        //UI service request
        public const string MMI_REQUEST_PROP_MESSAGE = "message";
        public const string MMI_REQUEST_PROP_ITEM = "item";
        //HTTP service request
        public const string MMI_REQUEST_PROP_REQ_METHOD = "requestMethod";
        public const string MMI_REQUEST_PROP_REQ_URL = "requestUrl";
        public const string MMI_REQUEST_PROP_REQ_HEADER_INFO = "requestHeaderInfo";
        public const string MMI_REQUEST_PROP_REQ_POST_BODY = "requestPostBody";
        public const string MMI_REQUEST_PROP_REQ_CONTENT_TYPE = "requestContentType";
        public const string MMI_REQUEST_PROP_REQ_FILE_INFO = "requestFileInformation";
        public const string MMI_REQUEST_PROP_REQ_FILE_TO_SAVE_NAME = "requestFileNameToSave";
        public const string MMI_REQUEST_PROP_HTTP_HEADER_KEY = "headerKey";
        public const string MMI_REQUEST_PROP_HTTP_HEADER_VALUE = "headerValue";

        //Persistence service request
        public const string MMI_REQUEST_PROP_STORE_NAME = "storeName";
        public const string MMI_REQUEST_PROP_PERSIST_REQ_DATA = "requestData";
        public const string MMI_REQUEST_PROP_PERSIST_KEY = "key";
        public const string MMI_REQUEST_PROP_PERSIST_VALUE = "value";
        //Database service request
        public const string MMI_REQUEST_PROP_APP_DB = "appDB";
        public const string MMI_REQUEST_PROP_QUERY_REQUEST = "queryRequest";
        //Map service request
        public const string MMI_REQUEST_PROP_LOCATIONS = "locations";
        public const string MMI_REQUEST_PROP_LEGENDS = "legends";
        public const string MMI_REQUEST_PROP_LOC_LATITUDE = "locLatitude";
        public const string MMI_REQUEST_PROP_LOC_LONGITUDE = "locLongitude";
        public const string MMI_REQUEST_PROP_LOC_MARKER = "locMarkerPin";
        public const string MMI_REQUEST_PROP_LOC_TITLE = "locTitle";
        public const string MMI_REQUEST_PROP_LOC_DESCRIPTION = "locDescription";
        //File read service
        public const string MMI_REQUEST_PROP_FILE_TO_READ_NAME = "fileName";
        public const string MMI_REQUEST_PROP_FOLDER_FILE_READ_FORMAT = "fileFormatToRead";
        public const string MMI_REQUEST_PROP_FOLDER_READ_SUBFOLDER = "readFilesInSubfolders";
        //Camera service
        public const string MMI_REQUEST_PROP_CAMERA_DIR = "cameraDirection";
        public const string MMI_REQUEST_PROP_IMG_COMPRESSION = "imageCompressionLevel";
        public const string MMI_REQUEST_PROP_IMG_ENCODING = "imageEncoding";
        public const string MMI_REQUEST_PROP_IMG_RETURN_TYPE = "imageReturnType";
        public const string MMI_REQUEST_PROP_IMG_FILTER = "imageFilter";
        public const string MMI_REQUEST_PROP_IMG_SRC = "imageSource";

        // Location service
        public const string MMI_REQUEST_PROP_LOC_ACCURACY = "locAccuracy";
        public const string MMI_REQUEST_PROP_LOCATION_TIMEOUT = "locTimeout";
        public const string MMI_REQUEST_PROP_LOCATION_LASTKNOWN = "locLastKnown";

        //Push notification service
        public const string MMI_REQUEST_PROP_PN_SERVER_URL = "serverUrl";
        public const string MMI_REQUEST_PROP_PN_SENDER_ID = "senderId";
        public const string MMI_REQUEST_PROP_PN_CALLBACK_FUNCTION = "notificationCallbackFunction";

        //Service Response object properties

        //UI service response
        public const string MMI_RESPONSE_PROP_USER_SELECTION = "userSelection";
        public const string MMI_RESPONSE_PROP_USER_SELECTED_INDEX = "selectedIndex";
        //HTTP service response
        public const string MMI_RESPONSE_PROP_HTTP_RESPONSE_HEADERS = "httpResponseHeaders";
        public const string MMI_RESPONSE_PROP_HTTP_RESPONSE = "httpResponse";
        public const string MMI_RESPONSE_PROP_HTTP_HEADER_NAME = "headerName";
        public const string MMI_RESPONSE_PROP_HTTP_HEADER_VALUE = "headerValue";
        //Persistence service response
        public const string MMI_RESPONSE_PROP_STORE_NAME = "storeName";
        public const string MMI_RESPONSE_PROP_STORE_RETURN_DATA = "storeReturnData";
        public const string MMI_RESPONSE_PROP_STORE_KEY = "key";
        public const string MMI_RESPONSE_PROP_STORE_VALUE = "value";
        //Database service response
        public const string MMI_RESPONSE_PROP_APP_DB = "appDB";
        public const string MMI_RESPONSE_PROP_DB_RECORDS = "dbRecords";
        public const string MMI_RESPONSE_PROP_DB_ATTRIBUTE = "dbAttribute";
        public const string MMI_RESPONSE_PROP_DB_ATTR_VALUE = "dbAttrValue";
        //Map service response
        //File Read service response
        public const string MMI_RESPONSE_PROP_FILE_CONTENTS = "fileContents";
        public const string MMI_RESPONSE_PROP_FILE_NAME = "fileName";
        public const string MMI_RESPONSE_PROP_FILE_CONTENT = "fileContent";
        public const string MMI_RESPONSE_PROP_FILE_TYPE = "fileType";
        public const string MMI_RESPONSE_PROP_FILE_SIZE = "fileSize";
        public const string MMI_RESPONSE_PROP_FILE_UNARCHIVE_LOCATION = "fileUnarchiveLocation";
        public const string MMI_RESPONSE_PROP_FILE_ARCHIVE_LOCATION = "fileArchiveLocation";
        //Camera service response
        public const string MMI_RESPONSE_PROP_IMAGE_URL = "imageURL";
        public const string MMI_RESPONSE_PROP_IMAGE_DATA = "imageData";
        public const string MMI_RESPONSE_PROP_IMAGE_TYPE = "imageType";
        public const string MMI_RESPONSE_PROP_IS_CAMERA_OPERATION_SUCCESSFUL = "isCameraOperationSuccessful";
    }
}

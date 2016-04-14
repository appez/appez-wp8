using System;

namespace appez.exceptions
{
    /// <summary>
    /// ExceptionTypes : Defines all the exception types and respective messages that
    /// can occur while servicing the user request. These exception types and
    /// messages are communicated back to the user as a response of the operation
    /// request.
    /// </summary>
    public static class ExceptionTypes
    {
        //Exception types
        public static int UNKNOWN_EXCEPTION = -1;
        public static int SERVICE_TYPE_NOT_SUPPORTED_EXCEPTION = -2;
        public static int SMART_APP_LISTENER_NOT_FOUND_EXCEPTION = -3;
        public static int SMART_CONNECTOR_LISTENER_NOT_FOUND_EXCEPTION = -4;
        public static int INVALID_PAGE_URI_EXCEPTION = -5;
        public static int INVALID_PROTOCOL_EXCEPTION = -6;
        public static int INVALID_ACTION_CODE_PARAMETER = -7;
        public static int ACTION_CODE_NUMBER_FORMAT_EXCEPTION = -8;
        public static int IO_EXCEPTION = -9;
        public static int HTTP_PROCESSING_EXCEPTION = -10;
        public static int NETWORK_NOT_REACHABLE_EXCEPTION = -11;
        public static int FILE_NOT_FOUND_EXCEPTION = -12;
        public static int MALFORMED_URL_EXCEPTION = -13;
        public static int PROTOCOL_EXCEPTION = -14;
        public static int UNSUPPORTED_ENCODING_EXCEPTION = -15;
        public static int SOCKET_EXCEPTION_REQUEST_TIMED_OUT = -16;
        public static int ERROR_SAVE_DATA_PERSISTENCE = -17;
        public static int ERROR_RETRIEVE_DATA_PERSISTENCE = -18;
        public static int ERROR_DELETE_DATA_PERSISTENCE = -19;
        public static int JSON_PARSE_EXCEPTION = -20;
        public static int UNKNOWN_CURRENT_LOCATION_EXCEPTION = -21;
        public static int DB_OPERATION_ERROR = -22;
        public static int SOCKET_EXCEPTION = -23;
        public static int UNKNOWN_NETWORK_EXCEPTION = -24;
        public static int DEVICE_SUPPORT_EXCEPTION = -25;
        public static int FILE_READ_EXCEPTION = -26;
        public static int PROBLEM_SAVING_IMAGE_TO_EXTERNAL_STORAGE_EXCEPTION = -28;
        public static int PROBLEM_CAPTURING_IMAGE_EXCEPTION = -29;
        public static int ERROR_RETRIEVING_CURRENT_LOCATION = -30;
        public static int CAMERA_NOT_AVAILABLE = -100;
        public static int MEMORY_NOT_AVAILABLE = -101;
        public static int SPACE_NOT_AVAILABLE = -102;
        public static int FILE_UNZIP_ERROR = -32;
        public static int FILE_ZIP_ERROR = -33;
        public static int DB_OPEN_ERROR = -34;
        public static int DB_QUERY_EXEC_ERROR = -35;
        public static int DB_READ_QUERY_EXEC_ERROR = -36;
        public static int DB_TABLE_NOT_EXIST_ERROR = -37;
        public static int DB_CLOSE_ERROR = -38;
        public static int INVALID_SERVICE_REQUEST_ERROR = -39;
        public static int INVALID_JSON_REQUEST = -40;
        public static int LOCATION_ERROR_GPS_NETWORK_DISABLED = -41;
        public static int NOTIFIER_REQUEST_INVALID = -43;
        public static int NOTIFIER_REQUEST_ERROR = -44;

        //EXCEPTION MESSAGE
        public static string NETWORK_NOT_REACHABLE_EXCEPTION_MESSAGE = "Network not reachable";
        public static string UNABLE_TO_PROCESS_MESSAGE = "Unable to process request";
        public static string UNKNOWN_CURRENT_LOCATION_EXCEPTION_MESSAGE = "Could not get current location";
        public static string JSON_PARSE_EXCEPTION_MESSAGE = "Unable to parse JSON";
        public static string HARDWARE_CAMERA_IN_USE_EXCEPTION_MESSAGE = "Camera already in use";
        public static string PROBLEM_CAPTURING_IMAGE_EXCEPTION_MESSAGE = "Problem capturing image from camera";
        public static string ERROR_RETRIEVING_CURRENT_LOCATION_MESSAGE = "Unable to retrieve current location";
        public static string ERROR_DELETE_DATA_PERSISTENCE_MESSAGE = "Problem deleting data from persistence store";
        public static string ERROR_RETRIEVE_DATA_PERSISTENCE_MESSAGE = "Problem retrieving data from persistence store";
        public static string ERROR_SAVE_DATA_PERSISTENCE_MESSAGE = "Problem saving data to persistence store";
        public static string DB_OPERATION_ERROR_MESSAGE = "Problem performing database operation";
        public static string FILE_UNZIP_ERROR_MESSAGE = "Unable to extract the archive file.";
        public static string FILE_ZIP_ERROR_MESSAGE = "Unable to create archive file.";
        public static string INVALID_SERVICE_REQUEST_ERROR_MESSAGE = "Invalid Service Request. Make sure that you have provided all the required parameters in the request.";
        public static string LOCATION_ERROR_GPS_NETWORK_DISABLED_MESSAGE = "Could not fetch the location.GPS radio or Network disabled";
        public static string NOTIFIER_REQUEST_INVALID_MESSAGE = "Notifier request invalid.";
        public static string NOTIFIER_REQUEST_ERROR_MESSAGE = "Error processing notifier request";

    }
}

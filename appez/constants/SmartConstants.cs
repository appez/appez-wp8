using System;
using System.IO;

namespace appez.constants
{
    /// <summary>
    /// SmartConstants: Holds all the constants used throughout the framework. These
    /// constants are not specific to any operation.
    /// </summary>
    public class SmartConstants
    {

        public const string APP_NAME = "appez";
        public const string PAGE_URI = "key_url";
        public const string KEY_APP_PAGE_URL = "APP_PAGE_URL";
        public const string REQUEST_DATA = "Http_request_data";
        public const string CREATE_FILE_DUMP = "Is_file_dump_required";
        public const string FILE_LOCATION = "file_location";
        public const string CHECK_FOR_INTENT_EXTRA = "check_for_intent_extra";
        public const string CHECK_FOR_BACKGROUND = "is_show_background";
        public const string CHECK_FOR_APP_CONFIG_INFO = "app_config_information";
        public const string SHOW_APP_HEADER = "show_app_header";
        public const string HTTP_REQUEST_TYPE_POST = "POST";
        public const string HTTP_REQUEST_TYPE_GET = "GET";
        public const string HTTP_REQUEST_TYPE_PUT = "PUT";
        public const string HTTP_REQUEST_TYPE_DELETE = "DELETE";

        // Successful event completion notifications
        public const int NETWORK_REACHABLE = 2;

        public const int NATIVE_EVENT_BACK_PRESSED = 0;
        public const int NATIVE_EVENT_PAGE_INIT_NOTIFICATION = 1;
        public const int NATIVE_EVENT_ENTER_PRESSED = 2;
        public const int NATIVE_EVENT_ACTIONBAR_UP_PRESSED = 3;
        public const int NATIVE_EVENT_SOFT_KB_SHOW = 4;
        public const int NATIVE_EVENT_SOFT_KB_HIDE = 5;

        public const string NATIVE_EVENT_LIST_ITEM_SELECTED_SEPARATOR = "^";

        public const int MENU_OPTION_LOGOUT_ID = 100;

        public const string USER_SELECTION_YES = "0";
        public const string USER_SELECTION_NO = "1";
        public const string USER_SELECTION_OK = "2";
        public const string USER_SELECTION_CANCEL = "-1";


        public const string SEPARATOR_NEW_LINE = "\n";
        public const string HORIZONTAL_TAB = "\r";
        public const string MSG_REQUEST_HEADER_SPLIT = "##";
        public const string MSG_KEY_VALUE_SEPARATOR = "|";
        public const string ESCAPE_SEQUENCE_BACKSLASH_DOUBLEQUOTES = "\\\"";


        public const string MESSAGE_DIALOG_TITLE_TEXT_SEPARATOR = "~";
        public const string RETRIEVE_ALL_FROM_PERSISTENCE = "*";
        public const string FILE_TYPE_DAT = ".dat";

        public const int LOG_LEVEL_ERROR = 1;
        public const int LOG_LEVEL_DEBUG = 2;
        public const int LOG_LEVEL_INFO = 3;

        // To identify the response as XML or JSON
        public const string RESPONSE_TYPE_XML = "<?xml version";
        public const string RESPONSE_TYPE_XML_START_SYMBOL = "<";
        public const string RESPONSE_TYPE_XML_END_SYMBOL = ">";
        public const string JSON_RESPONSE_START_IDENTIFIER_OBJECT = "{";
        public const string JSON_RESPONSE_END_IDENTIFIER_OBJECT = "}";
        public const string JSON_RESPONSE_START_IDENTIFIER_ARRAY = "[";
        public const string JSON_RESPONSE_END_IDENTIFIER_ARRAY = "]";
        public const string REQUEST_TYPE_XML = "XML";
        public const string REQUEST_TYPE_JSON = "JSON";
        public const string WEB_ASSETS_LOCATION = "file:///android_asset/";
        public const string FILE_SYSTEM = "file:///";
        public const string FILE_TYPE_XML = "xml";

        public const string HEADER_TYPE_CONTENT_ENCODING = "Content-Encoding";
        public const string CONTENT_ENCODING_GZIP = "gzip";

        public const string REQUEST_TIMED_OUT = "The operation timed out";
        public const int REQUEST_SUCCESSFUL_EXECUTION = 0;

        public const string NEW_LINE_ENCODE = "%0A";
        public const string ENCODE_SINGLE_QUOTE_UTF8 = "%27";

        public const int HTTP_RESPONSE_STATUS_OK = 200;
        public const int HTTP_REQUEST_ELEMENT_LENGTH = 6;
        public const string HTTP_RESPONSE_HEADERS_ARRAY_NODE_NAME = "appez-responseHeaders";
        public const string HTTP_RESPONSE_HEADER_PROP_NAME = "appez-headerName";
        public const string HTTP_RESPONSE_HEADER_PROP_VALUE = "appez-headerValue";


        // GENERIC JSON RESPONSE PROPERTY TAGS
        public const string RESPONSE_JSON_PROP_DATA = "data";

        // Constants for Location service
        public const string LOCATION_RESPONSE_TAG_LATITUDE = "locationLatitude";
        public const string LOCATION_RESPONSE_TAG_LONGITUDE = "locationLongitude";

        public const string RESOURCE_CLASS_NAME_STRING = "string";
        public const string RESOURCE_CLASS_NAME_LAYOUT = "layout";
        public const string RESOURCE_CLASS_NAME_DRAWABLE = "drawable";
        public readonly string ASSETS_COMMON_IMAGES_LOCATION = "www" + Path.DirectorySeparatorChar + "app" + Path.DirectorySeparatorChar + "resources" + Path.DirectorySeparatorChar + "images" + Path.DirectorySeparatorChar + "commons"+ Path.DirectorySeparatorChar;
        // Constants required for Map component
        public const string GOOGLE_MAPS_API_KEY = "0sjWzstHRWTxzuea1VjalLkI7rjmHGIU7XMFHmA";
        public const string MESSAGE_CURRENT_LOCATION = "You are here";

        public const int MAP_DEFAULT_ZOOM_LEVEL = 12;
        public const string MAP_INTENT_MAP_CREATION_INFO = "MapCreationInfo";
        public const string MAP_INTENT_GET_DIRECTION_INFO = "MapGetDirectionInfo";
        public const string MAP_INTENT_SOURCE_LATITUDE = "SourceLatitude";
        public const string MAP_INTENT_SOURCE_LONGITUDE = "SourceLongitude";
        public const string MAP_INTENT_DESTINATION_LATITUDE = "DestinationLatitude";
        public const string MAP_INTENT_DESTINATION_LONGITUDE = "DestinationLongitude";
        public const string MAP_ERROR_MESSAGE_GETTING_DIRECTIONS = "Error getting directions for the specified pair of locations";

        // Request location updates after 0 minutes
        public const int MAP_LOCATION_UPDATES_MINIMUM_TIME = 0 * 60 * 1000;
        // Request location updates after change in distance over previous location
        // is 5KM
        public const int MAP_LOCATION_UPDATES_MINIMUM_DISTANCE = 1 * 1000;

        // Constants required for Application startup file

        public const string APP_STARTUP_INFO_NODE_ROOT = "appStart";
        // Information tags in Application startup JSON containing information
        // regarding dynamic menu creation
        public const string APP_STARTUP_INFO_NODE_MENUS = "menus";
        public const string MENUS_CREATION_PROPERTY_LABEL = "menuTitle";
        public const string MENUS_CREATION_PROPERTY_ICON = "menuIcon";
        public const string MENUS_CREATION_PROPERTY_ID = "menuId";
        public const string MENU_CREATION_INFO_SEPARATOR = "#";

        // TODO Add Information tags in Application startup JSON containing
        // information regarding tab creation
        public const string APP_STARTUP_INFO_NODE_TABS = "tabs";
        public const string TABS_CREATION_PROPERTY_LABEL = "tabLabel";
        public const string TABS_CREATION_PROPERTY_ICON = "tabIcon";
        public const string TABS_CREATION_PROPERTY_ID = "tabId";
        public const string TABS_CREATION_PROPERTY_CONTENT_URL = "tabContentUrl";
        public const string TABS_CREATION_INFO_SEPARATOR = "#";

        // Constants corresponding to the map JSON received from Javascript end
        public const string MAP_LEGEND_INFO_NODE = "punchInformation";

        public const string MAP_MARKER_RED = "0";
        public const string MAP_MARKER_GREEN = "1";
        public const string MAP_MARKER_BLUE = "2";
        public const string MAP_MARKER_YELLOW = "3";

        public const string CAMERA_FRONT = "CAMERA_FRONT";
        public const string CAMERA_BACK = "CAMERA_BACK";
        public const string IMAGE_URL = "IMAGE_URL";
        public const string IMAGE_DATA = "IMAGE_DATA";
        public const string IMAGE_JPEG = "JPEG";
        public const string IMAGE_PNG = "PNG";
        public const string IMAGE_GIF = "GIF";
        public const string IMAGE_FORMAT_SEPIA = "SEPIA";
        public const string IMAGE_FORMAT_MONOCHROME = "MONOCHROME";
        public const string IMAGE_FORMAT_STANDARD = "STANDARD";

        public const string FILE_DATA = "FILE_DATA";
        public const string FILE_URL = "FILE_URL";

        public const string ISO_FOLDER = "Images";



        // constant for event types
        public const int WEB_EVENT = 1;
        public const int CO_EVENT = 2;
        public const int APP_EVENT = 3;



        public const int MSG_KEY_LENGTH = 6;
        public const int ACTION_CODE_LENGTH = 5;



        //New appez.config property file property keys
        public const string APPEZ_CONF_PROP_MENU_INFO = "app.menuInfo";
        public const string APPEZ_CONF_PROP_TOPBAR_INFO = "app.topbar";
        public const string APPEZ_CONF_PROP_ACTIONBAR_ENABLE = "app.actionbarEnable";
        public static string APPEZ_CONF_PROP_PUSH_NOTIFIER_LISTENER = "app.pushNotifierListener";

        // Default timeout for location requests. Currently 2 minutes.
        public const int LOCATION_SERVICE_DEFAULT_TIMEOUT = 2 * 60 * 1000;

        // Location service constants
        public const string LOCATION_ACCURACY_COARSE = "coarse";
        public const string LOCATION_ACCURACY_FINE = "fine";
    }
}


namespace appez.constants
{
    /// <summary>
    /// [4][3][2][1][0]
    /// [4]th Place- Will signify Event type i.e. Web=1, CO=2 or App=3
    /// [3]rd and [2]nd place - Will signify service type like UI Service,HTTP Service, Database service etc.
    /// [1]st and [0]th place - Will signify service Action/operation type like show loader, hide loader, HTTP Action types etc.
    /// </summary>
    class WebEvents
    {
        // UI Service constants
        public const int WEB_SHOW_ACTIVITY_INDICATOR = 10101;
        public const int WEB_HIDE_ACTIVITY_INDICATOR = 10102;
        public const int WEB_UPDATE_LOADING_MESSAGE = 10103;
        public const int WEB_SHOW_DATE_PICKER = 10104;
        public const int WEB_SHOW_MESSAGE = 10105;
        public const int WEB_SHOW_MESSAGE_YESNO = 10106;
        public const int WEB_SHOW_INDICATOR = 10107;
        public const int WEB_HIDE_INDICATOR = 10108;

        public const int WEB_SHOW_DIALOG_SINGLE_CHOICE_LIST = 10109;
        public const int WEB_SHOW_DIALOG_SINGLE_CHOICE_LIST_RADIO_BTN = 10110;
        public const int WEB_SHOW_DIALOG_MULTIPLE_CHOICE_LIST_CHECKBOXES = 10111;


        // HTTP Service constants
        public const int WEB_HTTP_REQUEST = 10201;
        public const int WEB_HTTP_REQUEST_SAVE_DATA = 10202;

        // Data Persistence service constants
        public const int WEB_SAVE_DATA_PERSISTENCE = 10401;
        public const int WEB_RETRIEVE_DATA_PERSISTENCE = 10402;
        public const int WEB_DELETE_DATA_PERSISTENCE = 10403;

        // Database service constants
        public const int WEB_OPEN_DATABASE = 10501;
        public const int WEB_EXECUTE_DB_QUERY = 10502;
        public const int WEB_EXECUTE_DB_READ_QUERY = 10503;
        public const int WEB_CLOSE_DATABASE = 10504;

        //File Reading service constants
        public const int WEB_READ_FILE_CONTENTS = 10801;
        public const int WEB_READ_FOLDER_CONTENTS = 10802;
        public const int WEB_UNZIP_FILE_CONTENTS = 10803;
        public const int WEB_ZIP_CONTENTS = 10804;

        //Camera service constants
        public const int CAMERA_LAUNCH_CAMERA = 10901;
        public const int CAMERA_LAUNCH_GALLERY = 10902;

        //Soft upgrade service constants
        public const int WEB_SU_CHECK_UPGRADE = 11101;
        public const int WEB_SU_DOWNLOAD_ASSETS = 11102;
        public const int WEB_SU_EXTRACT_ASSETS = 11103;

        // Location service constants
	    public const int WEB_USER_CURRENT_LOCATION = 11001;

        

    }
}

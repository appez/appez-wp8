
namespace appez.constants
{
    /// <summary>
    /// Holds all the constants relating to notifiers and
    /// notifier related events and their communication constants.
    /// </summary>
    public class NotifierConstants
    {
        //The series for 
        public const int PUSH_MESSAGE_NOTIFIER = 1001;

        //List common action types for each notifier
        public static int NOTIFIER_ACTION_REGISTER = 1;
        public static int NOTIFIER_ACTION_UNREGISTER = 2;

        //Constants denoting properties related to server push request
        public static string PUSH_REGISTER_REQ_PROP_DEVICEID = "deviceId";
        public static string PUSH_REGISTER_REQ_PROP_APPID = "applicationId";
        public static string PUSH_REGISTER_REQ_PROP_APPNAME = "applicationName";
        public static string PUSH_REGISTER_REQ_PROP_APPVERSION = "appVersion";
        public static string PUSH_REGISTER_REQ_PROP_PLATFORM = "platform";
        public static string PUSH_REGISTER_REQ_PROP_PUSHID = "pushId";
    }
}

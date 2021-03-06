﻿
namespace appez.constants
{
    /// <summary>
    /// [4][3][2][1][0]
    /// [4]th Place- Will signify Event type i.e. Web=1, CO=2 or App=3
    /// [3]rd and [2]nd place - Will signify service type like UI Service,HTTP Service, Database service etc.
    /// [1]st and [0]th place - Will signify service Action/operation type like show loader, hide loader, HTTP Action types etc.
    /// </summary>
    public class AppEvents
    {
        public const int APP_NOTIFICATION = 30000;
	    // App Notification constants
        public const int APP_DEFAULT_ACTION = 30001;
	    public const int APP_NOTIFY_EXIT = 30002;
	    public const int APP_NOTIFY_MENU_ACTION = 30003;
        public const int APP_NOTIFY_DATA_ACTION = 30004;
        public const int APP_NOTIFY_ACTIVITY_ACTION = 30005;
        public const int APP_CONTROL_TRANSFER = 30006;
        public const int APP_NOTIFY_CREATE_TABS = 30007; 
	    public const int APP_NOTIFY_CREATE_MENU = 30008; 
	    public const int APP_MANAGE_STARTUP = 30009;
        public const int APP_NOTIFY_OPEN_BROWSER = 30010;
        
    }
}

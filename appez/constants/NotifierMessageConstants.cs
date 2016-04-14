
namespace appez.constants
{
    /// <summary>
    /// Holds all the constants used during web
    /// layer and native layer communication for notifier events request and response
    /// </summary>
    public class NotifierMessageConstants
    {
        //Standard request properties
        public static string NOTIFIER_REQUEST_PROP_CALLBACK_FUNC = "notifierCallback";
        public static string NOTIFIER_PROP_TRANSACTION_ID = "transactionId";
        public static string NOTIFIER_PROP_TRANSACTION_REQUEST = "notifierTransactionRequest";
        public static string NOTIFIER_PROP_TRANSACTION_RESPONSE = "notifierTransactionResponse";
        public static string NOTIFIER_TYPE = "notifierType";
        public static string NOTIFIER_ACTION_TYPE = "notifierActionType";
        public static string NOTIFIER_REQUEST_DATA = "notifierRequestData";
        public static string NOTIFIER_EVENT_RESPONSE = "notifierEventResponse";
        public static string NOTIFIER_OPERATION_IS_SUCCESS = "isOperationSuccess";
        public static string NOTIFIER_OPERATION_ERROR = "notifierError";
        public static string NOTIFIER_OPERATION_ERROR_TYPE = "notifierErrorType";

        //Push notifier constants
        public static string NOTIFIER_PUSH_PROP_SERVER_URL = "pushServerUrl";
        public static string NOTIFIER_PUSH_PROP_LOADING_MESSAGE = "loadingMessage";
        public static string NOTIFIER_REGISTER_ERROR_CALLBACK = "errorNotifierCallback";
        public static string NOTIFIER_REGISTER_ERROR_CALLBACK_SCOPE = "errorNotifierCallbackScope";

        //Standard response properties
        public static string NOTIFIER_PUSH_PROP_MESSAGE = "notifierPushMessage";
    }
}

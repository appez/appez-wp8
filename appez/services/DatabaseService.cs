using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace appez.services
{
    /// <summary>
    /// Provides access to the device database which is a SQLite
    /// implementation. Enables the user to create database that resides in the
    /// application sandbox. Also enables user to perform basic CRUD operations.
    /// Current implementation allows for execution of queries as they are provided
    /// by the user
    /// </summary>
    public class DatabaseService : SmartService
    {
        #region variables
        private SmartServiceListener smartServiceListener = null;
        private SqliteUtility sqliteUtility = null;
        private SmartEvent smartEvent = null;
        private string appDBName = null;
        #endregion
        public DatabaseService(SmartServiceListener smartServiceListener)
        {
            this.smartServiceListener = smartServiceListener;
        }


        public override void ShutDown()
        {
            this.smartServiceListener = null;
            sqliteUtility.Dispose();
            sqliteUtility = null;
        }
        /// <summary>
        /// Performs supported SQL operations
        /// <param name="smartEvent">Smart event</param>
        public override void PerformAction(SmartEvent smartEvent)
        {
            this.smartEvent = smartEvent;
            bool dbOperationResponse = false;
            JObject serviceRequestData = smartEvent.SmartEventRequest.ServiceRequestData;
            
            try
            {
                this.appDBName = serviceRequestData.GetValue(CommMessageConstants.MMI_RESPONSE_PROP_APP_DB).ToString();
                if (sqliteUtility == null)
                {
                    sqliteUtility = new SqliteUtility(this.appDBName);
                }
                JToken tempToken = null;
                switch (smartEvent.GetServiceOperationId())
                {
                    case WebEvents.WEB_OPEN_DATABASE:
                        
                        dbOperationResponse = sqliteUtility.OpenDatabase();

                        if (dbOperationResponse)
                        {
                            OnSuccessDatabaseOperation(PrepareResponse());
                        }
                        else
                        {
                            OnErrorDatabaseOperation(ExceptionTypes.DB_OPERATION_ERROR, ExceptionTypes.DB_OPERATION_ERROR_MESSAGE);
                        }
                        break;

                    case WebEvents.WEB_EXECUTE_DB_QUERY:
                        String queryString = null;

                        if (serviceRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_QUERY_REQUEST, out tempToken))
                        {
                            queryString = tempToken.ToString();
                        }
                        dbOperationResponse = sqliteUtility.ExecuteDbQuery(queryString);
                        if (dbOperationResponse)
                        {
                            OnSuccessDatabaseOperation(PrepareResponse());
                        }
                        else
                        {
                            OnErrorDatabaseOperation(ExceptionTypes.DB_OPERATION_ERROR, ExceptionTypes.DB_OPERATION_ERROR_MESSAGE);
                        }
                        break;

                    case WebEvents.WEB_EXECUTE_DB_READ_QUERY:
                        String readQueryString = null;
                        
                        if (serviceRequestData.TryGetValue(CommMessageConstants.MMI_REQUEST_PROP_QUERY_REQUEST, out tempToken))
                        {
                            readQueryString = tempToken.ToString();
                        }

                        String readQueryResponse = sqliteUtility.ExecuteReadTableQuery(readQueryString);
                        if (readQueryResponse != null)
                        {
                            OnSuccessDatabaseOperation(readQueryResponse);
                        }
                        else
                        {
                            OnErrorDatabaseOperation(sqliteUtility.QueryExceptionType, ExceptionTypes.DB_OPERATION_ERROR_MESSAGE);
                        }
                        break;
                    case WebEvents.WEB_CLOSE_DATABASE:
                        dbOperationResponse = sqliteUtility.CloseDatabase();
                        if (dbOperationResponse)
                        {
                            OnSuccessDatabaseOperation(PrepareResponse());
                        }
                        else
                        {
                            OnErrorDatabaseOperation(ExceptionTypes.DB_OPERATION_ERROR, ExceptionTypes.DB_OPERATION_ERROR_MESSAGE);
                        }
                        break;
                    default:
                        // TODO Need to check if the 'default' case needs to be handled
                        break;
                }
            }
            catch (Exception e)
            {
                OnErrorDatabaseOperation(ExceptionTypes.DB_OPERATION_ERROR, e.Message.ToString());
            }
            
        }
        /// <summary>
        /// Indicates the successful completion of the database operation. Forwards
	    /// the successful operation completion notification through
        /// <see cref="SmartServiceListener"/> so that the result that can be communicated
	    /// back to the web layer
        /// </summary>
        /// <param name="dbResponse">Prepared response received from the database utility</param>
        private void OnSuccessDatabaseOperation(string dbResponse)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete=true;
            smEventResponse.ServiceResponse=dbResponse;
            smEventResponse.ExceptionType=0;
            smEventResponse.ExceptionMessage=null;
            smartEvent.SmartEventResponse=smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }
        /// <summary>
        /// Indicates that the database operation could not complete successfully.
	    /// Also specifies the type of exception and its description when forwarding
        /// the completion notification through <see cref="SmartServiceListener"/> .
        /// </summary>
        /// <param name="exceptionData">Exception type.</param>
        /// <param name="exceptionMessage">Exception message.</param>
        private void OnErrorDatabaseOperation(int exceptionType, String exceptionMessage)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = false;
            smEventResponse.ServiceResponse = null;
            smEventResponse.ExceptionType = exceptionType;
            smEventResponse.ExceptionMessage = exceptionMessage;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(smartEvent);
        }
        /// <summary>
        /// Stringifies the JSON response object for database operation completion
        /// </summary>
        /// <returns><see cref="String"/></returns>
        private string PrepareResponse()
        {
            JObject dbResponseObj = new JObject();
            try
            {
                dbResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_APP_DB, this.appDBName);
            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION); 
            }

            return dbResponseObj.ToString();
        }

    }
}

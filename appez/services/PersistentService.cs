using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace appez.services
{
    /// <summary>
    /// Allows the user to hold data in isolated storage for holding
    /// data across application's session. This service
    /// allows saving data in key-value pair, retrieving data and deleting data on
    /// the basis of key
    /// </summary>
    public class PersistentService : SmartService
    {
        #region variables
        private SmartEvent smartEvent = null;
        private SmartServiceListener smartServiceListener = null;
        private StorePreferencesUtility storePref = null;
        #endregion

        /// <summary>
        /// Creates the instance of PersistenceService
        /// </summary>
        /// <param name="smartServiceListener">SmartServiceListener</param>
        public PersistentService(SmartServiceListener smartServiceListener)
        {
            this.smartServiceListener = smartServiceListener;
        }

        public override void ShutDown()
        {

            smartServiceListener = null;
        }


        public override void PerformAction(SmartEvent smEvent)
        {
            this.smartEvent = smEvent;

            storePref = new StorePreferencesUtility();
            
            try
            {
                switch (smEvent.GetServiceOperationId())
                {
                    case WebEvents.WEB_SAVE_DATA_PERSISTENCE:
                        SaveData(smEvent.SmartEventRequest.ServiceRequestData);
                        break;

                    case WebEvents.WEB_RETRIEVE_DATA_PERSISTENCE:
                        RetrieveData(smEvent.SmartEventRequest.ServiceRequestData);
                        break;

                    case WebEvents.WEB_DELETE_DATA_PERSISTENCE:
                        DeleteData(smEvent.SmartEventRequest.ServiceRequestData);
                        break;
                }
            }
            catch (Exception rte)
            {
                OnErrorPersistenceOperation(ExceptionTypes.UNKNOWN_EXCEPTION, rte.Message.ToString());

            }
        }
        /// <summary>
        /// Save the data in the persistence storage in the form of key-value pair.
        /// </summary>
        /// <param name="dataString">String containing list of key-value pairs that the user
        /// wants to save</param>
        private void SaveData(JObject reqObject)
        {
            try
            {
                bool preferenceSet = false;
                storePref.SetPreferenceName(reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_STORE_NAME).ToString());
                JArray requestData = (JArray)reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_REQ_DATA);
                if (requestData != null && requestData.Count > 0)
                {
                    foreach (var item in requestData.Children())
                    {
                        JObject keyValuePairObj = item.ToObject<JObject>();
                        
                        preferenceSet = storePref.SetPreference(keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_KEY).ToString(),
                                keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_VALUE).ToString());
                        if (!preferenceSet)
                        {
                            break;
                        }
                        
                    }
                   
                    if (preferenceSet)
                    {
                        JObject storeResponseObj = new JObject();
                        storeResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_NAME, reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_STORE_NAME.ToString()));
                        storeResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_RETURN_DATA, null);
                        OnSuccessPersistenceOperation(storeResponseObj.ToString());
                    }
                    else
                    {
                        OnErrorPersistenceOperation(ExceptionTypes.ERROR_SAVE_DATA_PERSISTENCE, ExceptionTypes.ERROR_SAVE_DATA_PERSISTENCE_MESSAGE);
                    }
                }
            }
            catch (JsonException)
            {
                OnErrorPersistenceOperation(ExceptionTypes.ERROR_SAVE_DATA_PERSISTENCE, ExceptionTypes.ERROR_SAVE_DATA_PERSISTENCE_MESSAGE);
            }
            
        }
        /// <summary>
        /// Retrieve data from the isolated storage based on the key provided by the
        /// user.
        /// </summary>
        /// <param name="retrieveFilter"></param>
        private void RetrieveData(JObject reqObject)
        {
            try
            {
                storePref.SetPreferenceName(reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_STORE_NAME).ToString());
                JArray requestData = (JArray)reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_REQ_DATA);
                if (requestData != null && requestData.Count > 0)
                {
                    JArray storeResponseData = new JArray();
                    foreach (var item in requestData.Children())
                    {
                        JObject keyValuePairObj = item.ToObject<JObject>();

                        if (keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_KEY).ToString().Equals(SmartConstants.RETRIEVE_ALL_FROM_PERSISTENCE))
                        {
                            storeResponseData = GetAllFromSharedPreference();
                            break;
                        }
                        else
                        {
                            JObject responseElement = new JObject();
                            responseElement.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_KEY, keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_KEY).ToString());
                            responseElement.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_VALUE,
                            storePref.GetPreference(keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_KEY).ToString()));
                            storeResponseData.Add(responseElement);
                        }

                    }
                    
                    JObject storeResponseObj = new JObject();
                    storeResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_NAME, reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_STORE_NAME).ToString());
                    storeResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_RETURN_DATA, storeResponseData);
                    OnSuccessPersistenceOperation(storeResponseObj.ToString());
                }
            }
            catch (JsonException)
            {
                OnErrorPersistenceOperation(ExceptionTypes.ERROR_RETRIEVE_DATA_PERSISTENCE, ExceptionTypes.ERROR_RETRIEVE_DATA_PERSISTENCE_MESSAGE);
            }

        }
        /// <summary>
        /// Deleted the specified key from the persistent store. 
        /// </summary>
        /// <param name="deleteFilter">The string that specifies the key to be deleted from store.
        /// Returns the remaining keys in the store as a response to the
        /// web layer</param>
        private void DeleteData(JObject reqObject)
        {
            try
            {
                bool isDeleteFromPersistence = false;
                storePref.SetPreferenceName(reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_STORE_NAME).ToString());
                JArray requestData = (JArray)reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_REQ_DATA);
               
                if (requestData != null && requestData.Count > 0)
                {
                    foreach (var item in requestData.Children())
                    {
                        JObject keyValuePairObj = item.ToObject<JObject>();
                        isDeleteFromPersistence = storePref.RemoveFromPreference(keyValuePairObj.GetValue(CommMessageConstants.MMI_REQUEST_PROP_PERSIST_KEY).ToString());
                        if (!isDeleteFromPersistence)
                        {
                            break;
                        }

                    }
                    
                    if (isDeleteFromPersistence)
                    {
                        JObject storeResponseObj = new JObject();
                        storeResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_NAME, reqObject.GetValue(CommMessageConstants.MMI_REQUEST_PROP_STORE_NAME));
                        storeResponseObj.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_RETURN_DATA, null);
                        OnSuccessPersistenceOperation(storeResponseObj.ToString());
                    }
                    else
                    {
                        OnErrorPersistenceOperation(ExceptionTypes.ERROR_DELETE_DATA_PERSISTENCE, ExceptionTypes.ERROR_DELETE_DATA_PERSISTENCE_MESSAGE);
                    }
                }
            }
            catch (InvalidCastException)
            {
                OnErrorPersistenceOperation(ExceptionTypes.ERROR_DELETE_DATA_PERSISTENCE, ExceptionTypes.ERROR_DELETE_DATA_PERSISTENCE_MESSAGE);
            }
            catch (JsonException)
            {
                OnErrorPersistenceOperation(ExceptionTypes.ERROR_DELETE_DATA_PERSISTENCE, ExceptionTypes.ERROR_DELETE_DATA_PERSISTENCE_MESSAGE);
            }
        }
        /// <summary>
        /// Specifies the action to be taken when the persistence service operation
        /// is complete successfully
        /// </summary>
        /// <param name="successData">Contains the data received on completion of persistence
        /// operation</param>
        private void OnSuccessPersistenceOperation(string successData)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete = true;
            smEventResponse.ServiceResponse = successData;
            smEventResponse.ExceptionType = 0;
            smEventResponse.ExceptionMessage = null;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);
        }
        /// <summary>
        /// Specifies the action to be taken when the persistence service operation
        /// is complete unsuccessfully
        /// </summary>
        /// <param name="exceptionType">Unique code specifiying the type of error in the persistence
        /// operation</param>
        /// <param name="exceptionMessage">exception message</param>
        private void OnErrorPersistenceOperation(int exceptionType, String exceptionMessage)
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
        /// Helps retrieve all the saved keys from the persistent store.
        /// </summary>
        /// <returns>String containing all the keys and their corresponding
        /// values from the persistent store</returns>
        private JArray GetAllFromSharedPreference()
        {
            JArray responseArray = new JArray();
            try
            {
                Dictionary<string, string> allPreferenceEntries = storePref.GetAllFromPreference();

                if (allPreferenceEntries.Count > 0)
                {
                    foreach (KeyValuePair<string, string> entry in allPreferenceEntries)
                    {

                        JObject responseElement = new JObject();
                        responseElement.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_KEY, entry.Key);
                        responseElement.Add(CommMessageConstants.MMI_RESPONSE_PROP_STORE_VALUE, entry.Value);
                        responseArray.Add(responseElement);

                    }

                }

            }
            catch (InvalidCastException)
            {
                responseArray = null;
            }
            catch (JsonException)
            {
                responseArray = null;
            }
            catch (Exception)
            {
                responseArray = null;
            }
            return responseArray;
            
        }

    }
}


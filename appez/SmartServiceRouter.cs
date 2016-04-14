using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.services;
using System;
using System.Collections.Generic;


namespace appez
{
    /// <summary>
    /// It is based on factory pattern. It maintains the
    /// pool of Services, on new requests it checks the availability of request in
    /// pool. If it is available then it uses the reference else it creates the
    /// object of new services makes entry in Pool and then returns the object of
    /// service.
    /// </summary>
    public class SmartServiceRouter
    {
        #region variables
        private Dictionary<String, SmartService> servicesSet = null;
        private SmartServiceListener smartServiceListener = null;
        #endregion
        public SmartServiceRouter()
        {

        }

        public SmartServiceRouter(SmartServiceListener smartServiceListener)
        {
            this.servicesSet = new Dictionary<String, SmartService>();
            this.smartServiceListener = smartServiceListener;
        }

        /// <summary>
        /// Returns instance of SmartService based on the service type
        /// </summary>
        /// <param name="serviceType">Type of service</param>
        /// <returns>SmartService</returns>
        public SmartService GetService(int serviceType)
        {
            SmartService smartService = null;
            String key = Convert.ToString(serviceType);

            if (servicesSet.ContainsKey(key))
            {
                smartService = servicesSet[key];
            }
            else
            {
                switch (serviceType)
                {
                    case ServiceConstants.UI_SERVICE:
                        smartService = new UIService(smartServiceListener);
                        break;

                    case ServiceConstants.HTTP_SERVICE:
                        smartService = new HttpService(smartServiceListener);
                        break;

                    case ServiceConstants.DATA_PERSISTENCE_SERVICE:
                        smartService = new PersistentService(smartServiceListener);
                        break;

                    case ServiceConstants.DEVICE_DATABASE_SERVICE:
                        smartService = new DatabaseService(smartServiceListener);
                        break;

                    case ServiceConstants.FILE_SERVICE:
                        smartService = new FileService(smartServiceListener);

                        break;

                    case ServiceConstants.MAPS_SERVICE:
                        smartService = new MapService(smartServiceListener);
                        break;

                    case ServiceConstants.CAMERA_SERVICE:
                        smartService = new CameraService(smartServiceListener);
                        break;

                    case ServiceConstants.LOCATION_SERVICE:
                        smartService = new LocationService(smartServiceListener);
                        break;

                    default:
                        throw new MobiletException(ExceptionTypes.SERVICE_TYPE_NOT_SUPPORTED_EXCEPTION);
                }

                if (smartService != null)
                {
                    servicesSet.Add(key, smartService);
                }
            }
            return smartService;
        }

        /// <summary>
        /// Removes the mentioned service from services set
        /// </summary>
        /// <param name="serviceType">Type of service</param>
        /// <param name="isEventCompleted">Indicates whether or not the event is complete</param>
        public void ReleaseService(int serviceType, bool isEventCompleted)
        {
            SmartService smartService = null;
            String key = Convert.ToString(serviceType);
            if (servicesSet.ContainsKey(key) && isEventCompleted)
            {
                smartService = servicesSet[key];
                smartService.ShutDown();
                servicesSet.Remove(key);
            }
        }

    }
}

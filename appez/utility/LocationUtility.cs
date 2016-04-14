using appez.constants;
using appez.exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace appez.utility
{
    /// <summary>
    /// Utility class for preparing response object for location service.
    /// </summary>
    class LocationUtility
    {

        /// <summary>
        /// Prepares a well formatted JSON response that contains the location
	    /// details to be sent to the web layer
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public static string PrepareLocationResponse(Geocoordinate location)
        {
            String locationResponse = null;
            try
            {
                JObject locationResponseObj = new JObject();
                if (location != null)
                {
                    locationResponseObj.Add(SmartConstants.LOCATION_RESPONSE_TAG_LATITUDE, location.Latitude.ToString());
                    locationResponseObj.Add(SmartConstants.LOCATION_RESPONSE_TAG_LONGITUDE, location.Longitude.ToString());
                }
                else
                {
                    locationResponseObj.Add(SmartConstants.LOCATION_RESPONSE_TAG_LATITUDE, "");

                    locationResponseObj.Add(SmartConstants.LOCATION_RESPONSE_TAG_LONGITUDE, "");
                }
                locationResponse = locationResponseObj.ToString();

            }
            catch (JsonException)
            {
                throw new MobiletException(ExceptionTypes.JSON_PARSE_EXCEPTION);
            }

            return locationResponse;
        }
    }
}


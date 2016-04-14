using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.model
{
    /// <summary>
    /// Singleton class to hold object to share between class.
    /// </summary>
    public class SessionData
    {
        private static SessionData sharedInstance;
        private bool isMapBusy;

        private SessionData()
        {
        }
        /// <summary>
        /// Return instance of class.
        /// </summary>
        /// <returns></returns>
        public static SessionData GetInstance()
        {
            if (sharedInstance == null)
            {
                sharedInstance = new SessionData();
            }
            return sharedInstance;
        }

        public bool GetIsMapBusy()
        {
            return this.isMapBusy;
        }

        public void SetIsMapBusy(bool value)
        {
            this.isMapBusy = value;
        }
    }
}

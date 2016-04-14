using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez.utility
{
    /// <summary>
    /// Indicates the reachability of the network in the
    /// device. Could be any of WiFi or cellular connection
    /// </summary>
    public class NetworkReachabilityUtility
    {
        private static NetworkReachabilityUtility reachability = null;

        private NetworkReachabilityUtility()
        {

        }

        public static NetworkReachabilityUtility GetInstance()
        {
            if (reachability == null)
            {
                reachability = new NetworkReachabilityUtility();
            }
            return reachability;
        }

        /// <summary>
        /// Checks for availability of Network
        /// </summary>
        /// <returns></returns>
        public bool CheckForConnection()
        {

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                NetworkInterfaceType nit = NetworkInterface.NetworkInterfaceType;
                Debug.WriteLine("Network availability:" + nit.ToString());
                if (nit != NetworkInterfaceType.None)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}

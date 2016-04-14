using appez.constants;
using appez.listeners;
using appez.listeners.notifier;
using appez.notifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appez
{
    /// <summary>
    /// It is based on factory pattern. it creates the
    /// object of new notifier and returns the object of notifier.
    /// </summary>
    public class NotifierEventRouter
    {

        private NotifierEventListener notifierEventListener = null;

        public NotifierEventRouter(NotifierEventListener notifierEvListener)
        {
            this.notifierEventListener = notifierEvListener;
        }
        /// <summary>
        /// Returns instance of NotifierEvent based on the notifier type
        /// </summary>
        /// <param name="notifierType"></param>
        /// <returns></returns>
        public SmartNotifier GetNotifier(int notifierType)
        {
            SmartNotifier smartNotifier = null;
            switch (notifierType)
            {
                case NotifierConstants.PUSH_MESSAGE_NOTIFIER:
                    smartNotifier = new PushMessageNotifier(this.notifierEventListener);
                    break;

                default:
                    break;
            }

            return smartNotifier;
        }
    }
}

using appez.model;
using Newtonsoft.Json.Linq;

namespace appez.notifier
{
    /// <summary>
    /// Generic class for notifier event.
    /// </summary>
    public abstract class SmartNotifier
    {
        public abstract void RegisterListener(NotifierEvent notifierEvent);
        public abstract void UnregisterListener(NotifierEvent notifierEvent);
    }
}

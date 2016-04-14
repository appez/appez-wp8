
namespace appez.listeners
{
    /// <summary>
    /// Define an interface to listen appbar's menu item click.
    /// </summary>
    public interface SmartNotificationListener
    {   
        /// <summary>
        /// Notify to javascript on menu item click.
        /// </summary>
        /// <param name="actionName"></param>
        void SendActionInfo(string actionName);
    }
}

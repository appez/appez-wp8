
namespace appez.listeners
{
    /// <summary>
    /// Defines an Interface for capturing the events from the UI
    /// dialog. This includes listening to events on decision dialog, single list
    /// selection dialog, multiple choice selection dialog, date pickers and other
    /// standard UI components
    /// </summary>
    public interface DialogListener
    {
        
        /// <summary>
        /// Specifies action to be taken on the basis of user selection provided
        /// </summary>
        /// <param name="userSelection">User selection</param>
        void ProcessUsersSelection(string userSelection);
    }
}

using TitheSync.Core.Models;

namespace TitheSync.Core.Stores
{
    /// <summary>
    ///     Stores and manages notifications.
    /// </summary>
    public class NotificationStore:INotificationStore
    {
        /// <summary>
        ///     Gets or sets the list of notifications.
        /// </summary>
        public List<Notification> Notifications { get; set; } = [];

        /// <summary>
        ///     Adds a notification to the store.
        /// </summary>
        /// <param name="notification" >The notification to add.</param>
        public void AddNotification( Notification notification )
        {
            Notifications.Add(notification);
            NotificationsChanged?.Invoke();
        }

        /// <summary>
        ///     Clears all notifications from the store.
        /// </summary>
        public void ClearNotifications()
        {
            Notifications.Clear();
            NotificationsChanged?.Invoke();
        }

        /// <summary>
        ///     Event triggered when notifications change.
        /// </summary>
        public event Action? NotificationsChanged;
    }
}

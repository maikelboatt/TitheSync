using TitheSync.Core.Models;

namespace TitheSync.Core.Stores
{
    public interface INotificationStore
    {
        List<Notification> Notifications { get; set; }

        void AddNotification( Notification notification );

        void ClearNotifications();

        event Action? NotificationsChanged;
    }
}

using TitheSync.Infrastructure.Models;

namespace TitheSync.ApplicationState.Stores
{
    public interface INotificationStore
    {
        List<Notification> Notifications { get; set; }

        void AddNotification( Notification notification );

        void ClearNotifications();

        event Action? NotificationsChanged;
    }
}

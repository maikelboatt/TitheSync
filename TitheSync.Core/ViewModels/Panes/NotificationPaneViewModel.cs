using MvvmCross.Commands;
using MvvmCross.ViewModels;
using TitheSync.ApplicationState.Stores;
using TitheSync.Infrastructure.Models;

namespace TitheSync.Core.ViewModels.Panes
{
    public class NotificationPaneViewModel:MvxViewModel
    {
        private readonly INotificationStore _notificationStore;
        private MvxObservableCollection<Notification> _notifications = [];


        public NotificationPaneViewModel( INotificationStore notificationStore )
        {
            _notificationStore = notificationStore;


            _notifications.CollectionChanged += ( sender, args ) =>
            {
                // Handle collection changed event if needed
                RaisePropertyChanged(() => Notifications);
            };
            _notificationStore.NotificationsChanged += OnNotificationsChanged;
        }

        public MvxObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public IMvxCommand ClearCommand => new MvxCommand(ClearCommandExecute);

        public override Task Initialize()
        {
            _notifications = new MvxObservableCollection<Notification>(_notificationStore.Notifications);
            return base.Initialize();
        }


        private void OnNotificationsChanged()
        {
            RaisePropertyChanged(() => Notifications);
        }

        private void ClearCommandExecute()
        {
            _notificationStore.ClearNotifications();
            _notifications.Clear();
        }
    }
}

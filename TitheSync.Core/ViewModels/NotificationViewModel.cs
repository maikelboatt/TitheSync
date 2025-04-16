using MvvmCross.ViewModels;
using TitheSync.Core.ViewModels.Panes;

namespace TitheSync.Core.ViewModels
{
    public class NotificationViewModel:MvxViewModel
    {
        public NotificationPaneViewModel? NotificationPaneViewModel { get; set; }
    }
}

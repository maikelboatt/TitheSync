using MvvmCross.ViewModels;
using TitheSync.Core.ViewModels.Panes;

namespace TitheSync.Core.ViewModels
{
    public class SettingsViewModel:MvxViewModel
    {
        public SettingsPaneViewModel? SettingsPaneViewModel { get; set; }
    }
}

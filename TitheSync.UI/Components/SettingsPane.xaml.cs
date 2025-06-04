using MvvmCross.Platforms.Wpf.Views;
using System.Windows;
using TitheSync.UI.Themes;

namespace TitheSync.UI.Components
{
    public partial class SettingsPane:MvxWpfView
    {
        public SettingsPane()
        {
            InitializeComponent();
        }

        private void Themes_Click( object sender, RoutedEventArgs e )
        {
            ThemesController.SetTheme(Themes.IsChecked == true ? ThemeTypes.Dark : ThemeTypes.Light);
        }
    }
}

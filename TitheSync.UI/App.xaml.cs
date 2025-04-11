using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace TitheSync.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App :MvxApplication
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<Setup>();
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}


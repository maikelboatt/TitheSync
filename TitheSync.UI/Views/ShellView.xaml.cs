using MvvmCross.Platforms.Wpf.Views;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TitheSync.UI.Views
{
    public partial class ShellView:MvxWpfView
    {
        public ShellView()
        {
            InitializeComponent();
        }

        // Start: Button Close | Restore | Minimize 
        private void BtnClose_Click( object sender, RoutedEventArgs e )
        {
            Window? parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void BtnRestore_Click( object sender, RoutedEventArgs e )
        {
            Window? parentWindow = Window.GetWindow(this);
            if (parentWindow != null) parentWindow.WindowState = parentWindow is { WindowState: WindowState.Normal } ? WindowState.Maximized : WindowState.Normal;
        }

        private void BtnMaximize_Click( object sender, RoutedEventArgs e )
        {
            Window? parentWindow = Window.GetWindow(this);
            if (parentWindow != null) parentWindow.WindowState = parentWindow is { WindowState: WindowState.Normal } ? WindowState.Maximized : WindowState.Normal;
        }

        private void BtnMinimize_Click( object sender, RoutedEventArgs e )
        {
            Window? parentWindow = Window.GetWindow(this);
            if (parentWindow != null) parentWindow.WindowState = WindowState.Minimized;
        }
        // End: Button Close | Restore | Minimize

        private void BG_PreviewMouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            TgBtn.IsChecked = false;
        }

        // Start: MenuLeft PopupButton //
        private void btnHome_MouseEnter( object sender, MouseEventArgs mouseEventArgs )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnHome;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Home";
        }

        private void btnHome_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnDashboard_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnDashboard;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Dashboard";
        }

        private void btnDashboard_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }


        private void btnBilling_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnBilling;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Payments";
        }

        private void btnBilling_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnOrderList_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnOrderList;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Notifications";
        }

        private void btnOrderList_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnSetting_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnSetting;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            Header.PopupText.Text = "Settings";
        }

        private void btnSetting_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }
        // End: MenuLeft PopupButton //
    }
}

using System.Windows;

namespace TitheSync.Business.Services
{
    public interface IMessageService
    {
        MessageBoxResult Show( string message, string caption, MessageBoxButton button, MessageBoxImage icon );
    }
}

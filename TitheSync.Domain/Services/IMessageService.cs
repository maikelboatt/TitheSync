using System.Windows;

namespace TitheSync.Domain.Services
{
    public interface IMessageService
    {
        MessageBoxResult Show( string message, string caption, MessageBoxButton button, MessageBoxImage icon );
    }
}

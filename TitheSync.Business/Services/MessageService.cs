using System.Windows;

namespace TitheSync.Business.Services
{
    public class MessageService:IMessageService
    {
        public MessageBoxResult Show( string message, string caption, MessageBoxButton button, MessageBoxImage icon ) => MessageBox.Show(message, caption, button, icon);
    }
}

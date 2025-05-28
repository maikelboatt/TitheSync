namespace TitheSync.Core.Models
{
    /// <summary>
    ///     Represents a notification with a title, message, timestamp, and type.
    /// </summary>
    public class Notification
    {
        /// <summary>
        ///     Gets or sets the title of the notification.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the message content of the notification.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the timestamp when the notification was created.
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        ///     Gets or sets the type of the notification.
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }
}

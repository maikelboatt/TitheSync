using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace TitheSync.Business.Services.Errors
{
    /// <summary>
    ///     Handles database-related errors and displays user-friendly messages.
    /// </summary>
    /// <param name="messageService" >Service for displaying messages to the user.</param>
    /// <param name="logger" >Logger for error logging.</param>
    public class DatabaseErrorHandlerService( IMessageService messageService, ILogger<DatabaseErrorHandlerService> logger ):IDatabaseErrorHandlerService
    {
        /// <summary>
        ///     Executes a database operation and handles exceptions, logging errors and showing messages to the user.
        /// </summary>
        /// <typeparam name="TResult" >The result type of the operation.</typeparam>
        /// <param name="operation" >The asynchronous database operation to execute.</param>
        /// <param name="userActionDescription" >A description of the user action being performed.</param>
        /// <returns>The result of the operation, or default if an error occurs.</returns>
        public async Task<TResult?> HandleDatabaseOperationAsync<TResult>( Func<Task<TResult>> operation, string userActionDescription )
        {
            try
            {
                return await operation();
            }
            catch (SqlException ex)
            {
                logger.LogError("Database connection failed during: {UserActionDescription}.", userActionDescription);
                // SQL error codes for login/permission issues: 18456 (login failed), 4060 (cannot open database)
                if (ex.Number is 18456 or 4060)
                {
                    messageService.Show(
                        "You do not have permission to perform this action. Please contact your administrator.",
                        "Permission Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                else
                {
                    messageService.Show(
                        "Unable to connect to the database. Please check your network or contact support.",
                        "Database Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

            }
            catch (TimeoutException ex)
            {
                logger.LogError("Network timeout occurred during: {UserActionDescription}", userActionDescription);
                messageService.Show(
                    "Unable to connect to the database. Please check your network or contact support.",
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected error during {Action}.", userActionDescription);
                messageService.Show(
                    "An unexpected error occurred. Please try again or contact support.",
                    "Unexpected Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            return default;
        }

        /// <summary>
        ///     Executes a database operation that does not return a value, handling exceptions and user notifications.
        /// </summary>
        /// <param name="operation" >The asynchronous database operation to execute.</param>
        /// <param name="userActionDescription" >A description of the user action being performed.</param>
        public async Task HandleDatabaseOperationAsync( Func<Task> operation, string userActionDescription )
        {
            await HandleDatabaseOperationAsync(
                async () =>
                {
                    await operation();
                    return true;
                },
                userActionDescription);

        }
    }
}

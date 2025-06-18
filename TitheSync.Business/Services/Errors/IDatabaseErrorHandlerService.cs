namespace TitheSync.Business.Services.Errors
{
    /// <summary>
    ///     Defines methods for handling database operations with error handling and user action context.
    /// </summary>
    public interface IDatabaseErrorHandlerService
    {
        /// <summary>
        ///     Executes a database operation asynchronously, handling any errors and providing context for the user action.
        /// </summary>
        /// <typeparam name="TResult" >The type of the result returned by the operation.</typeparam>
        /// <param name="operation" >The asynchronous database operation to execute.</param>
        /// <param name="userActionDescription" >A description of the user action being performed.</param>
        /// <returns>The result of the operation, or null if an error occurred.</returns>
        Task<TResult?> HandleDatabaseOperationAsync<TResult>( Func<Task<TResult>> operation, string userActionDescription );

        /// <summary>
        ///     Executes a database operation asynchronously, handling any errors and providing context for the user action.
        /// </summary>
        /// <param name="operation" >The asynchronous database operation to execute.</param>
        /// <param name="userActionDescription" >A description of the user action being performed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleDatabaseOperationAsync( Func<Task> operation, string userActionDescription );
    }
}

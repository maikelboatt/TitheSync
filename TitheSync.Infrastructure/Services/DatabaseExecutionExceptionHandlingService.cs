using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TitheSync.Infrastructure.Services
{
    public class DatabaseExecutionExceptionHandlingService( ILogger<DatabaseExecutionExceptionHandlingService> logger )
        :IDatabaseExecutionExceptionHandlingService
    {
        /// <summary>
        ///     Executes a database operation with exception handling and logging.
        /// </summary>
        /// <typeparam name="T" >The type of the result returned by the operation.</typeparam>
        /// <param name="storedProcedure" >The name of the stored procedure being executed.</param>
        /// <param name="parameters" >The parameters to pass to the stored procedure.</param>
        /// <param name="operation" >The asynchronous operation to execute.</param>
        /// <returns>The result of the operation.</returns>
        /// <exception cref="TimeoutException" >Thrown when a network timeout occurs during the operation.</exception>
        /// <exception cref="SqlException" >Thrown when SQL error occurs during the operation.</exception>
        public async Task<T?> ExecuteWithExceptionHandlingAsync<T>( string storedProcedure, object parameters, Func<Task<T>> operation )
        {
            logger.LogInformation("Starting database operation: {StoredProcedure}", storedProcedure);

            Stopwatch stopwatch = Stopwatch.StartNew();

            T? result = await operation();
            stopwatch.Stop();
            logger.LogInformation("Completed database operation: {StoredProcedure} in {ElapsedMilliseconds}ms", storedProcedure, stopwatch.ElapsedMilliseconds);

            return result;
        }
    }
}

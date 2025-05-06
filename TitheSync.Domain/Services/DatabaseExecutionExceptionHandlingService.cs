using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TitheSync.Domain.Services
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
        public async Task<T> ExecuteWithExceptionHandlingAsync<T>( string storedProcedure, object parameters, Func<Task<T>> operation )
        {
            logger.LogInformation("Starting database operation: {StoredProcedure}", storedProcedure);

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                T result = await operation();
                stopwatch.Stop();
                logger.LogInformation("Completed database operation: {StoredProcedure} in {ElapsedMilliseconds}ms", storedProcedure, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (TimeoutException ex)
            {
                logger.LogError(ex, "Network timeout occurred during {StoredProcedure}", storedProcedure);
                throw;
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "SQL error occurred during {StoredProcedure}: {ErrorNumber} - {ErrorMessage}", storedProcedure, ex.Number, ex.Message);
                throw;
            }
        }
    }
}

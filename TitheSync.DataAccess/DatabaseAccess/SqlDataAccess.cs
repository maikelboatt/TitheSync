using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace TitheSync.DataAccess.DatabaseAccess
{
    /// <summary>
    ///     Provides methods to interact with a SQL database using Dapper.
    /// </summary>
    public class SqlDataAccess:ISqlDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<SqlDataAccess> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlDataAccess" /> class.
        /// </summary>
        /// <param name="connectionString" >The connection string to the database.</param>
        /// <param name="logger" >The logger instance for logging repository operations.</param>
        /// <exception cref="ArgumentNullException" >Thrown when the connection string is null or empty.</exception>
        public SqlDataAccess( string connectionString, ILogger<SqlDataAccess> logger )
        {
            _logger = logger;
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");
        }

        /// <summary>
        ///     Executes a stored procedure and retrieves a collection of results.
        /// </summary>
        /// <typeparam name="T" >The type of the result objects.</typeparam>
        /// <typeparam name="TU" >The type of the parameters object.</typeparam>
        /// <param name="storedProcedure" >The name of the stored procedure to execute.</param>
        /// <param name="parameters" >The parameters to pass to the stored procedure.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, containing the collection of results.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T, TU>( string storedProcedure, TU parameters, CancellationToken cancellationToken = default )
        {
            try
            {
                await using SqlConnection connection = new(_connectionString);
                return await connection.QueryAsync<T>(
                    new CommandDefinition(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
            }
            catch (Exception ex)
            {
                // Log the exception (replace with a logging framework)
                _logger.LogError("Error executing query: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Executes a stored procedure that does not return any results.
        /// </summary>
        /// <typeparam name="T" >The type of the parameters object.</typeparam>
        /// <param name="storedProcedure" >The name of the stored procedure to execute.</param>
        /// <param name="parameters" >The parameters to pass to the stored procedure.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CommandAsync<T>( string storedProcedure, T parameters, CancellationToken cancellationToken = default )
        {
            try
            {
                await using SqlConnection connection = new(_connectionString);
                await connection.ExecuteAsync(
                    new CommandDefinition(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
            }
            catch (Exception ex)
            {
                // Log the exception (replace with a logging framework)
                _logger.LogError("Error executing command: {ExMessage}", ex.Message);
                throw;
            }
        }
    }
}

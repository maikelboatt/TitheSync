namespace TitheSync.DataAccess.DatabaseAccess
{
    /// <summary>
    ///     Interface for SQL data access operations.
    /// </summary>
    public interface ISqlDataAccess
    {
        /// <summary>
        ///     Executes a query asynchronously and returns the result as an enumerable of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T" >The type of the result set.</typeparam>
        /// <typeparam name="TU" >The type of the parameters object.</typeparam>
        /// <param name="storedProcedure" >The name of the stored procedure to execute.</param>
        /// <param name="parameters" >The parameters to pass to the stored procedure.</param>
        /// <param name="connectionString" >The connection string to use. Defaults to "DefaultConnection".</param>
        /// <returns>
        ///     A task representing the asynchronous operation, containing the result set as an enumerable of type
        ///     <typeparamref name="T" />.
        /// </returns>
        Task<IEnumerable<T>> QueryAsync<T, TU>( string storedProcedure, TU parameters, string connectionString = "DefaultConnection" );

        /// <summary>
        ///     Executes a command asynchronously without returning a result.
        /// </summary>
        /// <typeparam name="T" >The type of the parameters object.</typeparam>
        /// <param name="storedProcedure" >The name of the stored procedure to execute.</param>
        /// <param name="parameters" >The parameters to pass to the stored procedure.</param>
        /// <param name="connectionString" >The connection string to use. Defaults to "DefaultConnection".</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CommandAsync<T>( string storedProcedure, T parameters, string connectionString = "DefaultConnection" );
    }
}

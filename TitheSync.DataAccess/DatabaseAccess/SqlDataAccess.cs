using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TitheSync.DataAccess.DatabaseAccess
{
    public class SqlDataAccess:ISqlDataAccess
    {
        public async Task<IEnumerable<T>> QueryAsync<T, TU>( string storedProcedure, TU parameters, string connectionString = "DefaultConnection" )
        {
            using IDbConnection connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task CommandAsync<T>( string storedProcedure, T parameters, string connectionString )
        {
            using IDbConnection connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}

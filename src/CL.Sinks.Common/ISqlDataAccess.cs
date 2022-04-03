using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CL.Sinks.Common
{
    public interface ISqlDataAccess
    {
        ConnectionSettings ConnectionSettings { get; }
        IDbConnection ActiveConnection { get; }

        /// <summary>
        /// Takes either CL.Sinks.Common.Connection type or string as a connection name (NOT CONNECTION STRING)
        /// </summary>
        /// <param name="connection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        IDbConnection SqlConnection<T>(T connection);

        Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, Connection connection = null);
        Task<List<T>> LoadFromSqlAsync<T>(string sql, Connection connection = null);
        Task<T> LoadFirstFromSqlAsync<T, T1>(string sql, T1 parameters, Connection connection = null);
        Task<T> LoadFirstFromSqlAsync<T>(string sql, Connection connection = null);

        Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, string connectionName);
        Task<List<T>> LoadFromSqlAsync<T>(string sql, string connectionName);
        Task<T> LoadFirstFromSqlAsync<T, T1>(string sql, T1 parameters, string connectionName);
        Task<T> LoadFirstFromSqlAsync<T>(string sql, string connectionName);

        Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, Connection connection = null);
        Task<List<T>> LoadFromStoredProcedureAsync<T>(string storedProcedure, Connection connection = null);
        Task<T> LoadFirstFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, Connection connection = null);
        Task<T> LoadFirstFromStoredProcedureAsync<T>(string storedProcedure, Connection connection = null);

        Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, string connectionName);
        Task<List<T>> LoadFromStoredProcedureAsync<T>(string storedProcedure, string connectionName);
        Task<T> LoadFirstFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, string
        connectionName);
        Task<T> LoadFirstFromStoredProcedureAsync<T>(string storedProcedure, string connectionName);


        void SaveFromSql<T>(string sql, T parameters, Connection connection = null);
        void SaveFromSql(string sql, Connection connection = null);

        Task SaveFromSqlAsync<T>(string sql, T parameters, Connection connection = null);
        Task SaveFromSqlAsync(string sql, Connection connection = null);

        void SaveFromSql<T>(string sql, T parameters, string connectionName);
        void SaveFromSql(string sql, string connectionName);

        Task SaveFromSqlAsync<T>(string sql, T parameters, string connectionName);
        Task SaveFromSqlAsync(string sql, string connectionName);

        void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, Connection connection = null);
        void SaveFromStoredProcedure(string storedProcedure, Connection connection = null);

        Task SaveFromStoredProcedureAsync<T>(string storedProcedure, T parameters, Connection connection = null);
        Task SaveFromStoredProcedureAsync(string storedProcedure, Connection connection = null);

        void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, string connectionName);
        void SaveFromStoredProcedure(string storedProcedure, string connectionName);

        Task SaveFromStoredProcedureAsync<T>(string storedProcedure, T parameters, string connectionName);
        Task SaveFromStoredProcedureAsync(string storedProcedure, string connectionName);
    }
}

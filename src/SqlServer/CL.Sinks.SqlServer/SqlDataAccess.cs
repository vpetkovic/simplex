using CL.Sinks.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CL.Sinks.SqlServer
{
    public class SqlDataAccess : ISqlDataAccess, IDisposable
    {
        private readonly ConnectionSettings _connectionSettings;
        public ConnectionSettings ConnectionSettings => _connectionSettings;
        public IDbConnection ActiveConnection { get; private set; }

        public SqlDataAccess(ConnectionSettings connectionSettings)
        {
            _connectionSettings = connectionSettings;
            ActiveConnection = new SqlConnection(connectionSettings.DefaultConnection.ConnectionString);
        }

        public SqlDataAccess(Connection connection)
        {
            _connectionSettings = new ConnectionSettings(new Connection(connection.ConnectionString, "Default") { IsDefault = true, ConnectionTimeout = connection.ConnectionTimeout ?? 30 });
            ActiveConnection = new SqlConnection(connection.ConnectionString);
        }
        
        public IDbConnection SqlConnection<T>(T connection)
        {
            if (!(connection is Connection || connection is string) && connection != null)
                throw new ArgumentException("Invalid type specified", nameof(connection));

            Connection conn = new Connection();
            if (connection is Connection)
                conn = connection as Connection;
            else if (connection is string && !string.IsNullOrEmpty(connection.ToString()))
                conn = _connectionSettings.GetConnectionStringByName(connection.ToString()) ?? throw new ArgumentException ("Unable to find any of the specified connection names");
            else
                conn = _connectionSettings.DefaultConnection;

            conn.ConnectionTimeout ??= _connectionSettings.GlobalConnectionTimeout;
            ActiveConnection = new SqlConnection(conn.ConnectionString);
            return ActiveConnection;
        }

        #region Load From Sql

        public async Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, Connection connection = null)
            => await SqlConnection(connection).LoadFrom<T>(sql, parameters);

        public async Task<List<T>> LoadFromSqlAsync<T>(string sql, Connection connection = null)
            => await SqlConnection(connection).LoadFrom<T>(sql);

        public async Task<T> LoadFirstFromSqlAsync<T, T1>(string sql, T1 parameters, Connection connection = null)
            => await SqlConnection(connection).LoadFirstFrom<T>(sql, parameters);

        public async Task<T> LoadFirstFromSqlAsync<T>(string sql, Connection connection = null)
            => await SqlConnection(connection).LoadFirstFrom<T>(sql);

        public async Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, string connectionName)
            => await SqlConnection(connectionName).LoadFrom<T>(sql, parameters);

        public async Task<List<T>> LoadFromSqlAsync<T>(string sql, string connectionName)
            => await SqlConnection(connectionName).LoadFrom<T>(sql);

        public async Task<T> LoadFirstFromSqlAsync<T, T1>(string sql, T1 parameters, string connectionName)
            => await SqlConnection(connectionName).LoadFirstFrom<T>(sql, parameters);

        public async Task<T> LoadFirstFromSqlAsync<T>(string sql, string connectionName)
            => await SqlConnection(connectionName).LoadFirstFrom<T>(sql);

        #endregion

        #region Load From Stored Procedure

        public async Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, Connection connection = null)
            => await SqlConnection(connection).LoadFrom<T>(storedProcedure, parameters, true);

        public async Task<List<T>> LoadFromStoredProcedureAsync<T>(string storedProcedure, Connection connection = null)
            => await SqlConnection(connection).LoadFrom<T>(storedProcedure, isStoredProcedure:true);

        public async Task<T> LoadFirstFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, Connection connection = null)
            => await SqlConnection(connection).LoadFirstFrom<T>(storedProcedure, parameters, true);

        public async Task<T> LoadFirstFromStoredProcedureAsync<T>(string storedProcedure, Connection connection = null)
            => await SqlConnection(connection).LoadFirstFrom<T>(storedProcedure, isStoredProcedure: true);

        public async Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, string connectionName)
            => await SqlConnection(connectionName).LoadFrom<T>(storedProcedure, parameters, true);

        public async Task<List<T>> LoadFromStoredProcedureAsync<T>(string storedProcedure, string connectionName)
            => await SqlConnection(connectionName).LoadFrom<T>(storedProcedure, isStoredProcedure:true);

        public async Task<T> LoadFirstFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, string connectionName)
            => await SqlConnection(connectionName).LoadFirstFrom<T>(storedProcedure, parameters, true);

        public async Task<T> LoadFirstFromStoredProcedureAsync<T>(string storedProcedure, string connectionName)
            => await SqlConnection(connectionName).LoadFirstFrom<T>(storedProcedure, isStoredProcedure: true);

        #endregion

        #region Save From Sql

        public void SaveFromSql<T>(string sql, T parameters, Connection connection = null)
            => SqlConnection(connection).SaveFrom(sql, parameters);

        public void SaveFromSql(string sql, Connection connection = null)
            => SqlConnection(connection).SaveFrom(sql);

        public async Task SaveFromSqlAsync<T>(string sql, T parameters, Connection connection = null)
            => await SqlConnection(connection).SaveFromAsync(sql, parameters);

        public async Task SaveFromSqlAsync(string sql, Connection connection = null)
            => await SqlConnection(connection).SaveFromAsync(sql);

        public void SaveFromSql<T>(string sql, T parameters, string connectionName)
            => SqlConnection(connectionName).SaveFrom(sql, parameters);

        public void SaveFromSql(string sql, string connectionName)
            => SqlConnection(connectionName).SaveFrom(sql);

        public async Task SaveFromSqlAsync<T>(string sql, T parameters, string connectionName)
            => await SqlConnection(connectionName).SaveFromAsync(sql, parameters);

        public async Task SaveFromSqlAsync(string sql, string connectionName)
            => await SqlConnection(connectionName).SaveFromAsync(sql);

        #endregion

        #region Save From Stored Procedure

        public void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, Connection connection = null)
            => SqlConnection(connection).SaveFrom(storedProcedure, parameters, true);

        public void SaveFromStoredProcedure(string storedProcedure, Connection connection = null)
            => SqlConnection(connection).SaveFrom(storedProcedure, isStoredProcedure:true);

        public async Task SaveFromStoredProcedureAsync<T>(string storedProcedure, T parameters, Connection connection = null)
            => await SqlConnection(connection).SaveFromAsync(storedProcedure, parameters, true);

        public async Task SaveFromStoredProcedureAsync(string storedProcedure, Connection connection = null)
            => await SqlConnection(connection).SaveFromAsync(storedProcedure, isStoredProcedure:true);

        public void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, string connectionName)
            => SqlConnection(connectionName).SaveFrom(storedProcedure, parameters, true);

        public void SaveFromStoredProcedure(string storedProcedure, string connectionName)
            => SqlConnection(connectionName).SaveFrom(storedProcedure, isStoredProcedure:true);

        public async Task SaveFromStoredProcedureAsync<T>(string storedProcedure, T parameters, string connectionName)
            => await SqlConnection(connectionName).SaveFromAsync(storedProcedure, parameters, true);

        public async Task SaveFromStoredProcedureAsync(string storedProcedure, string connectionName)
            => await SqlConnection(connectionName).SaveFromAsync(storedProcedure, isStoredProcedure:true);

        #endregion

        public void Dispose() => ActiveConnection.Dispose();

        
    }

}

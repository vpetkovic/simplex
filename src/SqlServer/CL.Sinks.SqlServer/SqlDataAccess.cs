using CL.Sinks.Common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CL.Sinks.SqlServer
{
    public class SqlDataAccess : ISqlDataAccess, IDisposable
    {
        private ConnectionSettings _connectionSettings { get; }
        public ConnectionSettings ConnectionSettings => _connectionSettings ?? null;
        public IDbConnection ActiveConnection { get; set; }

        public SqlDataAccess(ConnectionSettings connectionSettings = null) => _connectionSettings = connectionSettings;

        public SqlDataAccess(Connection connection) => _connectionSettings = new ConnectionSettings(new Connection { IsDefault = true, ConnectionString = connection.ConnectionString, ConnectionTimeout = connection.ConnectionTimeout ?? 30, Name = "Default" });

        public IDbConnection SqlConnection(string connectionName = "Default")
        {
            ActiveConnection = new SqlConnection(_connectionSettings.GetConnectionStringByName(connectionName).ConnectionString);
            return ActiveConnection;
        }

        public async Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, Connection connection = null)
        {
            var c = connection ?? _connectionSettings.DefaultConnection;
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                var data = await conn.QueryAsync<T>(sql, parameters, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout).ConfigureAwait(true);

                return data.ToList();
            }
        }
        public async Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, string connectionName)
        {
            var c = _connectionSettings.GetConnectionStringByName(connectionName) ?? throw new ArgumentException("Unable to find any of the specified connection names");
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                var data = await conn.QueryAsync<T>(sql, parameters, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout).ConfigureAwait(true);

                return data.ToList();
            }
        }

        public async Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, Connection connection = null)
        {
            var c = connection ?? _connectionSettings.DefaultConnection;
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                var data = await conn.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout).ConfigureAwait(true);

                return data.ToList();
            }
        }
        public async Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, string connectionName)
        {
            var c = _connectionSettings.GetConnectionStringByName(connectionName) ?? throw new ArgumentException("Unable to find any of the specified connection names");
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                var data = await conn.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout).ConfigureAwait(true);

                return data.ToList();
            }
        }

        public void SaveFromSql<T>(string sql, T parameters, Connection connection = null)
        {
            var c = connection ?? _connectionSettings.DefaultConnection;
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                conn.Execute(sql, parameters, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout);
            }
        }
        public void SaveFromSql<T>(string sql, T parameters, string connectionName)
        {
            var c = _connectionSettings.GetConnectionStringByName(connectionName) ?? throw new ArgumentException("Unable to find any of the specified connection names");
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                conn.Execute(sql, parameters, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout);
            }
        }

        public void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, Connection connection = null)
        {
            var c = connection ?? _connectionSettings.DefaultConnection;
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                conn.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout);
            }
        }
        public void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, string connectionName)
        {
            var c = _connectionSettings.GetConnectionStringByName(connectionName) ?? throw new ArgumentException("Unable to find any of the specified connection names");
            using (IDbConnection conn = new SqlConnection(c.ConnectionString))
            {
                conn.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure, commandTimeout: c.ConnectionTimeout ?? _connectionSettings.GlobalConnectionTimeout);
            }
        }

        public void Dispose() => ActiveConnection.Dispose();
    }

}

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CL.Sinks.Common
{
    public interface ISqlDataAccess
    {
        ConnectionSettings ConnectionSettings { get; }
        IDbConnection ActiveConnection { get; }
        IDbConnection SqlConnection(string connectionName = "Default");
        Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, Connection connection = null);
        Task<List<T>> LoadFromSqlAsync<T, T1>(string sql, T1 parameters, string connectionName);
        Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, Connection connection = null);
        Task<List<T>> LoadFromStoredProcedureAsync<T, T1>(string storedProcedure, T1 parameters, string connectionName);
        void SaveFromSql<T>(string sql, T parameters, Connection connection = null);
        void SaveFromSql<T>(string sql, T parameters, string connectionName);
        void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, Connection connection = null);
        void SaveFromStoredProcedure<T>(string storedProcedure, T parameters, string connectionName);
    }
}

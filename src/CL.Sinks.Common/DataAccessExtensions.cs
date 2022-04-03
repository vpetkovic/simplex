using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace CL.Sinks.Common
{
    public static class DataAccessExtensions
    {
        public static async Task<List<T>> LoadFrom<T>(
            this IDbConnection connection,
            string toRun,
            object parameters = null,
            bool isStoredProcedure = false)
        {
            CommandType commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            object p = parameters ?? new { };

            using (IDbConnection conn = connection)
            {
                return (await conn.QueryAsync<T>(toRun, p, commandType: commandType)).ToList();
            }
        }

        /// <summary>
        /// Result: No Item -> Default; One Item -> Item; Many Items -> First Item
        /// </summary>
        public static async Task<T> LoadFirstFrom<T>(
            this IDbConnection connection,
            string toRun,
            object parameters = null,
            bool isStoredProcedure = false)
        {
            CommandType commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            object p = parameters ?? new { };

            using (IDbConnection conn = connection)
            {
                return await conn.QueryFirstOrDefaultAsync<T>(toRun, p, commandType: commandType);
            }
        }

        public static void SaveFrom(
            this IDbConnection connection,
            string toRun,
            object parameters = null,
            bool isStoredProcedure = false)
        {
            CommandType commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            object p = parameters ?? new { };

            using (IDbConnection conn = connection)
            {
                conn.Execute(toRun, p, commandType: commandType);
            }
        }

        public static async Task SaveFromAsync(
            this IDbConnection connection,
            string toRun,
            object parameters = null,
            bool isStoredProcedure = false)
        {
            CommandType commandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text;
            object p = parameters ?? new { };

            using (IDbConnection conn = connection)
            {
                await conn.ExecuteAsync(toRun, p, commandType: commandType);
            }

        }
    }
}
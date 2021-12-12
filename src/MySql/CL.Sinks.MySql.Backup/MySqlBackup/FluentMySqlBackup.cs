using CL.Sinks.Common;

namespace CL.Sinks.MySql.Backup
{
    public sealed partial class FluentMySqlBackup
    {
        private readonly Connection _connection;
        private int _defaultConnectionTimeout = 0;

        private FluentMySqlBackup(Connection connection) => _connection = connection;

        public static IMySqlServer For(Connection connection) => new FluentMySqlBackup(connection);
    }
}

using System;

namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    public class DatabaseListModel
    {
        public string DatabaseName { get; set; }
        public string BackupFileName => !string.IsNullOrEmpty(DatabaseName) ? $"{DatabaseName}_{DateTime.Now.ToString("yyyy_MM_dd_hhmm")}.sql" : null;
        public bool Export { get; set; } = false;
    }

}

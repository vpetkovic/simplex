using CL.Sinks.MySql.Backup.MySqlConnector.Storages;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    /// <summary>
    /// Export and Import 
    /// </summary>
    public sealed partial class FluentMySqlBackup : IMySqlServer
    {
        private string[] _databases;
        private List<DatabaseListModel> _databaseList;
        private BackupStatus _backupStatus = new BackupStatus();

        #region Export
        public IMySqlServerActions Export(string[] databases)
        {
            if (databases == null || databases.Count() == 0) throw new ArgumentNullException($"{nameof(databases)} parameter cannot be null or contain no elements");

            _databases = databases;
            _databaseList = new List<DatabaseListModel>();

            if (!_backupStatus.IsRunning) _backupStatus.Start();

            using (var conn = new MySqlConnection(_connection.GetConnectionString))
            {
                using (var cmd = new MySqlCommand("SHOW DATABASES", conn))
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandTimeout = _connection.ConnectionTimeout ?? _defaultConnectionTimeout;
                        Console.WriteLine($"Started retrieving the list of databases on server");
                        var reader = cmd.ExecuteReader();

                        int ii = 0;
                        while (reader.Read())
                        {
                            string row = "";
                            for (int i = 0; i < reader.FieldCount; i++) { row += reader.GetFieldValue<string>(i).ToString(); ii++; }

                            if (_databases.Contains(row) || _databases[0].Equals("all", StringComparison.InvariantCultureIgnoreCase))
                                _databaseList.Add(new DatabaseListModel { DatabaseName = row, Export = true });
                            else _databaseList.Add(new DatabaseListModel { DatabaseName = row, Export = false });

                        }
                        conn.Close();
                        Console.WriteLine($"Total of {ii} database{(ii > 1 ? "s" : "")} retrieved. Only {_databaseList.Count(e=>e.Export)} database{(_databaseList.Count > 1 ? "s" : "")} eligible for backup");
                    }
                    catch (MySqlException ex)
                    {
                        conn.Close();
                        _backupStatus.AddBackupError(nameof(Export), ex);
                        Console.WriteLine(ex);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Internal Helper Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="options"></param>
        private void ExportFor<T>(DatabaseListModel x, T options)
        {
            using (var conn = new MySqlConnection(_connection.GetConnectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    conn.Open();
                    cmd.CommandTimeout = _connection.ConnectionTimeout ?? _defaultConnectionTimeout;
                    try
                    {
                        using (var mb = new MySqlBackup(cmd))
                        {
                            Console.WriteLine($"Started backing up '{x.DatabaseName}' database");
                            conn.ChangeDatabase(x.DatabaseName);
                            cmd.Connection = conn;

                            using (MemoryStream ms = new MemoryStream())
                            {
                                mb.ExportToMemoryStream(ms);
                                Console.WriteLine($"Backup Generated for '{x.DatabaseName}' database");

                                ms.Upload<T, DatabaseListModel>(options, x);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        _backupStatus.AddBackupError(nameof(Export), ex);
                        Console.WriteLine($"Backup failed: {ex}");

                    }
                    conn.CloseAsync();
                }
            }
        }

        /// <summary>
        /// Internal Helper Method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="multiThreaded"></param>
        private void ExecuteFor<T>(T options, bool multiThreaded = false)
        {
            var z = typeof(T).Name;
            if (multiThreaded && _databaseList.Count > 0) Parallel.ForEach(_databaseList.Where(e=>e.Export), x => ExportFor<T>(x, options));
            else _databaseList.Where(e => e.Export).ToList().ForEach(x => ExportFor<T>(x, options));
        }
        
        #endregion

        #region Import
        public IMySqlServerActions Import() => throw new NotImplementedException();

        public IMySqlServerActions FromFile(string backupName) => throw new NotImplementedException();
        #endregion
    }
}

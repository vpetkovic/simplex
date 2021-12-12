using CL.Sinks.MySql.Backup.MySqlConnector.Models;
using System;

namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    /// <summary>
    /// Action Options
    /// </summary>
    public sealed partial class FluentMySqlBackup : IMySqlServerActions
    {
        private AzureBlobStorageModel _azureBlobStorage;
        private LocalStorageModel _localStorageModel;

        public IAzureBlob ToAzure(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException($"{nameof(connectionString)} parameter cannot be null or empty");

            _azureBlobStorage = new AzureBlobStorageModel() { ConnectionString = connectionString, IsEnabled = true };

            return this;
        }

        public ILocalStorage ToLocalStorage(string storagePath)
        {
            if (string.IsNullOrEmpty(storagePath)) throw new ArgumentNullException(nameof(storagePath), "Parameter cannot be null or empty");

            _localStorageModel = new LocalStorageModel() { OutputPath = storagePath, IsEnabled = true };

            return this;
        }

        IMySqlServerActions IMySqlServerActions.FromFile(string backupName) => throw new NotImplementedException();

        public BackupStatus Done()
        {
            _backupStatus.Stop();

            // Disposing all objects and return backupSummary
            return _backupStatus;
        }


    }
}

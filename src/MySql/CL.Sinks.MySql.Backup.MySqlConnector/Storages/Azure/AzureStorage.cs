using System;

namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    /// <summary>
    /// Azure
    /// </summary>
    public sealed partial class FluentMySqlBackup : IAzureBlob, IAzureAction
    {
        public IAzureAction OnBlob(string blobBlockPath)
        {
            if (string.IsNullOrEmpty(blobBlockPath)) throw new ArgumentNullException($"{nameof(blobBlockPath)} parameter cannot be null or empty");
            _azureBlobStorage.ContainerName = blobBlockPath;
            return this;
        }

        public IMySqlServerActions Upload(bool multiThreaded = false) { ExecuteFor(_azureBlobStorage, multiThreaded); return this; }
        
    }
}

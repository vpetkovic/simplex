using System;

namespace CL.Sinks.MySql.Backup
{
    public interface IMySqlServer
    {
        /// <summary>
        /// Collect the list of databases to export, verify they exist on the given server and prepare them from export
        /// </summary>
        /// <param name="databases"></param>
        /// <exception cref="ArgumentException"></exception>
        IMySqlServerActions Export(string[] databases);

        /// <summary>
        /// Import databases to server
        /// </summary>
        /// <remarks>This is a placeholder | NOT YET IMPLEMENTED!</remarks>
        /// <exception cref="NotImplementedException"></exception>
        IMySqlServerActions Import();
    }

    public interface IMySqlServerActions
    {
        /// <exception cref="ArgumentException"></exception>
        IAzureBlob ToAzure(string connectionString);
        
        /// <exception cref="ArgumentException"></exception>
        ILocalStorage ToLocalStorage(string outputPath);

        /// <remarks>This is a placeholder | NOT YET IMPLEMENTED!</remarks>
        /// <exception cref="NotImplementedException"></exception>
        IMySqlServerActions FromFile(string backupName);

        /// <summary>
        /// Finalize backup process
        /// </summary>
        /// <param name="multiThreaded"></param>
        /// <returns></returns>
        BackupStatus Done();
    }


}

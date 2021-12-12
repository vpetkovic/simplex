using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using CL.Sinks.MySql.Backup.Models;

namespace CL.Sinks.MySql.Backup.Storages
{
    public static class StorageExtensions
    {

        /// <summary>
        /// Export database to all available and enabled storage types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="ms"></param>
        /// <param name="backupOptions"></param>
        /// <param name="db"></param>
        public static void Upload<T, D>(this MemoryStream ms, object backupOptions, D db)
            where D : DatabaseListModel, new()
        {
            if (backupOptions is AzureBlobStorageModel && backupOptions != null)
            {
                var b = (AzureBlobStorageModel)Convert.ChangeType(backupOptions, typeof(T));

                if (b.IsEnabled)
                {
                    Console.WriteLine("==============================");
                    Console.WriteLine("===== Azure Blob Storage =====");
                    Console.WriteLine("==============================");

                    Console.WriteLine("Connecting to Azure Blob Storage");
                    BlobServiceClient blobServiceClient = new BlobServiceClient(b.ConnectionString);
                    string containerName = b.ContainerName ?? db.DatabaseName;
                    BlobContainerClient containerClient;

                    Console.WriteLine($"Successfully connected to '{blobServiceClient.AccountName}'");

                    // Create a container if doesn't exists else laod existing one
                    if (!blobServiceClient.GetBlobContainers().Any(s => s.Name == containerName))
                    {
                        Console.WriteLine($"Container '{containerName}' doesn't exist -> Creating '{containerName}' container");
                        containerClient = blobServiceClient.CreateBlobContainer(containerName);
                        Console.WriteLine($"Container '{containerClient.Name}' created");
                        Console.WriteLine($"Using '{containerClient.Name}' container for {db.DatabaseName} database");
                    }
                    else
                    {
                        containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                        Console.WriteLine($"Using '{containerClient.Name}' container for {db.DatabaseName} database");
                    }

                    // Get a reference to a blob
                    BlobClient blobClient = containerClient.GetBlobClient($"{b.BackupDateTime.ToString("yyyy-MM-dd")}/{db.BackupFileName}");

                    Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
                    ms.Position = 0;
                    blobClient.Upload(ms, true);
                    Console.WriteLine("Upload complete");
                }
            }

            if (backupOptions is LocalStorageModel && backupOptions != null)
            {
                var b = (LocalStorageModel)Convert.ChangeType(backupOptions, typeof(T));

                if (b.IsEnabled && b.OutputPath != null)
                {
                    Console.WriteLine("==============================");
                    Console.WriteLine("======= Local Storage ========");
                    Console.WriteLine("==============================");
                    var destination = Path.Combine(b.OutputPath, db.BackupFileName);

                    Console.WriteLine($"Saving to {destination}");
                    ms.Position = 0;

                    using (FileStream fs = new FileStream(destination, FileMode.OpenOrCreate))
                    {
                        ms.CopyTo(fs);
                        fs.Flush();
                        Console.WriteLine("Save complete");
                    }
                }
            }
        }
    }
    public static class AzureKeyVaultExtensions
    {
        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                if (certificateCollection.Count == 0) throw new Exception("Certificate is not installed");

                return certificateCollection[0];

            }
            finally
            {
                store.Close();
            }
        }
    }
}

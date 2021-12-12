
namespace CL.Sinks.MySql.Backup.MySqlConnector.Models
{
    public class AzureBlobStorageModel : StorageBaseOptionModel, IAzureStorageOptionModel
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }

    public interface IAzureStorageOptionModel
    {
        string ConnectionString { get; set; }
        string ContainerName { get; set; }
    }
}

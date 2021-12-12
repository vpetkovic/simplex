
namespace CL.Sinks.MySql.Backup.Models
{
    public class LocalStorageModel : StorageBaseOptionModel, ILocalStorageOptionModel
    {
        public string OutputPath { get; set; }
    }

    public interface ILocalStorageOptionModel
    {
        string OutputPath { get; set; }
    }
}

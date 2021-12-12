using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Sinks.MySql.Backup.MySqlConnector.Models
{
    public interface IStorageBaseOptionModel
    {
        bool IsEnabled { get; set; }
        DateTime BackupDateTime { get; set; }
    }

    public class StorageBaseOptionModel : IStorageBaseOptionModel
    {
        public bool IsEnabled { get; set; } = false;
        public DateTime BackupDateTime { get; set; } = DateTime.Now;
    }





}

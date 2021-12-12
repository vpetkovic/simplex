using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Sinks.MySql.Backup
{
    public class BackupStatus
    {
        private IDictionary<string, object> _exceptions = new Dictionary<string, object>();
        private Stopwatch _stopwatch = new Stopwatch();

        public IDictionary<string, object> Exceptions => _exceptions;
        public bool IsSuccess => _exceptions.Count == 0 ? true : false;

        public decimal TotalBackupTimeMilliseconds => _stopwatch.ElapsedMilliseconds;

        public bool IsRunning => _stopwatch.IsRunning;

        public void Start() => _stopwatch.Start();
        public void Stop() => _stopwatch.Stop();

        public void AddBackupError(string database, Exception exception)
        {
            if (database == null) throw new ArgumentNullException("database");
            if (exception is Exception && exception is null) throw new ArgumentNullException("exception");
            _exceptions.Add(database, exception);
        }

        public void AddBackupError(string database, string errorMessage)
        {
            if (database == null) throw new ArgumentNullException("database");
            if (errorMessage == null) throw new ArgumentNullException("errorMessage");
            _exceptions.Add(database, errorMessage);
        }
    }
}

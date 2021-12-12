using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    public partial class Connection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        /// <summary>
        /// Converts the zero Datetime to NULL
        /// </summary>
        public bool ConvertZeroDateTime { get; set; } = false;
        public int? ConnectionTimeout { get; set; } = null;
        public bool IsDefault { get; set; } = false;

        public string GetConnectionString => !string.IsNullOrEmpty(ConnectionString)
            ? $"{ConnectionString}{(ConvertZeroDateTime ? ";convert zero datetime=True" : string.Empty)}"
            : throw new ArgumentException(nameof(ConnectionString), "Connection string is missing");
    }
}

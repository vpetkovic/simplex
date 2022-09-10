using System;

namespace CL.Sinks.Common
{
    public class Connection
    {
        public Connection() {}
        public Connection(string connectionString) => ConnectionString = connectionString;

        public Connection(string connectionString, string name)
        {
            ConnectionString = connectionString;
            Name = name;
        }
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        /// <summary>
        /// Converts the zero Datetime to NULL
        /// </summary>
        /// <remarks>For MySql Only</remarks>
        public bool ConvertZeroDateTime { get; set; } = false;
        public int? ConnectionTimeout { get; set; } = null;
        public bool IsDefault { get; set; } = false;

        public string GetConnectionString => !string.IsNullOrEmpty(ConnectionString)
            ? $"{ConnectionString}{(ConvertZeroDateTime ? ";convert zero datetime=True" : string.Empty)}"
            : throw new ArgumentException(nameof(ConnectionString), "Connection string is missing");
    }

}

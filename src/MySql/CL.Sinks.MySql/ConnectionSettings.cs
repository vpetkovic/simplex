using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CL.Sinks.Common
{
    public class ConnectionSettings
    {
        #region Private Members
        /// <summary>
        /// List of Database Connections. Take precedence over ConnectionStrings provided in config file
        /// </summary>
        private List<Connection> _connections { get; set; } = new List<Connection>();
        /// <summary>
        /// Provide config If connection stings are already defined in appsettings.json
        /// </summary>
        private IConfiguration _config = null;
        private List<Connection> GetConnections
        {
            get
            {
                if (_connections != null && _connections.Count == 0)
                {
                    _config
                        .GetSection("ConnectionStrings")?
                        .GetChildren()?.AsEnumerable()
                        .Where(s => !string.IsNullOrEmpty(s.Value)).ToList()
                        .ForEach(x => _connections.Add(new Connection
                        {
                            Name = x.Key,
                            ConnectionString = x.Value,
                            IsDefault = x.Key.Equals("default", StringComparison.InvariantCultureIgnoreCase) ? true : false,
                            ConnectionTimeout = GlobalConnectionTimeout
                        }));
                }
                return _connections;
            }
        }
        #endregion

        #region Constructors
        public ConnectionSettings(Connection connection)
        {
            if (connection is null) throw new ArgumentNullException($"{nameof(connection)} cannot be null.");

            _connections.Add(connection);
        } // => throw new ArgumentNullException($"Either config or list of connections must be provided.");
        public ConnectionSettings(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException($"{nameof(config)} cannot be null. At least one must be provided.");

            _config = config;
        }
        public ConnectionSettings(List<Connection> connections)
        {
            if (connections is null) throw new ArgumentNullException($"{nameof(_connections)} cannot be null. At least one must be provided.");

            _connections = connections;
        }
        #endregion

        #region Public Members
        public List<Connection> Connections => GetConnections;
        public int GlobalConnectionTimeout { get; set; } = 30;
        public Connection DefaultConnection => GetConnections?.FirstOrDefault(d => d.IsDefault) ?? throw new ArgumentException("Default connection is not set.");
        public Connection GetConnectionStringByName(string ConnectionName) => Connections?.Where(n => n.Name.Equals(ConnectionName)).FirstOrDefault();
        #endregion
    }

}

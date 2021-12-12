using CL.Sinks.Common;

namespace CL.Sinks.SqlServer.Demo
{
    public struct ConnectionStrings 
    {
        public static List<Connection> Connections = new List<Connection>()
        {
            new Connection
            {
                Name= "Default",
                ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                IsDefault = true,
            },
            new Connection
            {
                Name= "Secondary",
                ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                ConnectionTimeout = 120,
            }
        };
    }
}

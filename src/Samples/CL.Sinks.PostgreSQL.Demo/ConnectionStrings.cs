using CL.Sinks.Common;

namespace CL.Sinks.PostgreSQL.Demo;

public struct ConnectionStrings
{
    public static List<Connection> Connections = new List<Connection>()
    {
        new Connection("Server=192.168.50.117;Port=3785;User Id=root;Password=mypass;Database=postgres;", "Default") { IsDefault = true },
        new Connection
        {
            Name= "Secondary",
            ConnectionString = "Server=192.168.50.117;Port=3785;User Id=root;Password=mypass;Database=postgres;",
            ConnectionTimeout = 120,
        }
    };
}
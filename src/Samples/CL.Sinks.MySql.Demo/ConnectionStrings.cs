using CL.Sinks.Common;

namespace CL.Sinks.MySql.Demo
{
    public struct ConnectionStrings 
    {
        public static List<Connection> Connections = new List<Connection>()
        {
            new Connection("User Id=root;password=mypass;Host=192.168.50.117;Port=3306;", "Default") {IsDefault = true},
            new Connection()
            {
                Name = "Default",
                ConnectionString = "User Id=root;password=mypass;Host=192.168.50.117;Port=3306;",
                ConnectionTimeout = 120
            }
        };
    }
}

using CL.Sinks.Common;
using CL.Sinks.SqlServer;
using CL.Sinks.SqlServer.Demo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


#region Instatiations Examples

/// <summary>
/// Using Dependency Injection and connections specified in appsettings.json
/// </summary>
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, SqlDataAccess>(provider => new SqlDataAccess(new ConnectionSettings(config)))
    .BuildServiceProvider();

/// <summary>
/// Instatiating single connection 
/// </summary>
var singleConnection = new SqlDataAccess(ConnectionStrings.Connections.FirstOrDefault());

/// <summary>
/// Instatiating multiple connections alongside some global settings such as connection timeout
/// </summary>
var multipleConnections = new SqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)
{
    GlobalConnectionTimeout = 90,
});
#endregion

#region Query Examples

// Dependency Injection 
using var sp = serviceProvider.CreateScope();
var db = sp.ServiceProvider.GetRequiredService<ISqlDataAccess>();

// using Default connection 
var dbList1 = await db.LoadFromSqlAsync<string, dynamic>("SELECT name FROM sys.databases", new { });
var dbList2 = await singleConnection.LoadFromSqlAsync<string, dynamic>("SELECT name FROM sys.databases", new { });

// using other registered connections
var secondaryConnection = db.ConnectionSettings.GetConnectionStringByName("Secondary");
var dbList3 = await db.LoadFromSqlAsync<string, dynamic>("SELECT name FROM sys.databases", new { }, secondaryConnection);
var dbList4 = await multipleConnections.LoadFromSqlAsync<string, dynamic>("SELECT name FROM sys.databases", new { }, secondaryConnection);

#endregion
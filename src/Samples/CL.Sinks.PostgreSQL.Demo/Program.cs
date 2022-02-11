using CL.Sinks.PostgreSQL;
using CL.Sinks.PostgreSQL.Demo;
using CL.Sinks.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#region Instatiations Examples

// Using Dependency Injection and connections specified in appsettings.json
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, PostgreSqlDataAccess>(_ => new PostgreSqlDataAccess(new ConnectionSettings(config)))
    .BuildServiceProvider();


// Instantiating single connection
var singleConnection = new PostgreSqlDataAccess(ConnectionStrings.Connections.FirstOrDefault());

// Instantiating multiple connections alongside some global settings such as connection timeout
var multipleConnections = new PostgreSqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)
{
    GlobalConnectionTimeout = 90,
});
#endregion

#region Query Examples

// Dependency Injection
using var sp = serviceProvider.CreateScope();
var db = sp.ServiceProvider.GetRequiredService<ISqlDataAccess>();

// using Default connection
var query = "select * from postgres.public.users";
var dbList1 = await db.LoadFromSqlAsync<string, dynamic>(query, new {});
var dbList2 = await singleConnection.LoadFromSqlAsync<string, dynamic>(query, new { });

// using other registered connections
var secondaryConnection = db.ConnectionSettings.GetConnectionStringByName("Secondary");
var dbList3 = await db.LoadFromSqlAsync<string, dynamic>(query, new { }, secondaryConnection);
var dbList4 = await multipleConnections.LoadFromSqlAsync<string, dynamic>(query, new { }, secondaryConnection);
#endregion





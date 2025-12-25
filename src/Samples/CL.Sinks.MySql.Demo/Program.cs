using CL.Sinks.MySql;
using CL.Sinks.MySql.Demo;
using CL.Sinks.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.UserSecrets;

#region Instantiations Examples
/// <summary>
/// Using Dependency Injection and connections specified in appsettings.json
/// </summary>
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, MySqlDataAccess>(_ => new MySqlDataAccess(new ConnectionSettings(config)))
    .BuildServiceProvider();

/// <summary>
/// Instantiating single connection
/// </summary>
var singleConnection = new MySqlDataAccess(ConnectionStrings.Connections.FirstOrDefault());

/// <summary>
/// Instantiating multiple connections alongside some global settings such as connection timeout
/// </summary>
var multipleConnections = new MySqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)
{
    GlobalConnectionTimeout = 90,
});
#endregion

#region Query Examples

// Dependency Injection 
using var sp = serviceProvider.CreateScope();
var db = sp.ServiceProvider.GetRequiredService<ISqlDataAccess>();

// using Default connection
var query = "show databases";
var dbList1 = await db.LoadFromSqlAsync<string>(query);
var single = await db.LoadFirstFromSqlAsync<string>(query);
var dbList2 = await singleConnection.LoadFromSqlAsync<string, dynamic>("show databases", new { });

// using other registered connections
var secondaryConnection = db.ConnectionSettings.GetConnectionStringByName("Secondary");
var dbList3 = await db.LoadFromSqlAsync<string>(query, secondaryConnection);
var dbList4 = await multipleConnections.LoadFromSqlAsync<string>("show databases", secondaryConnection);

await db.SaveFromSqlAsync("INSERT INTO Persons (Name) Values (@Name);", new[]
{
    new {Name = "John"}, new {Name = "Jane"}
});

#endregion
using CL.Sinks.Common;
using CL.Sinks.SqlServer;
using CL.Sinks.SqlServer.Demo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


#region Instantiations Examples

/// <summary>
/// Using Dependency Injection and connections specified in appsettings.json
/// </summary>
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, SqlDataAccess>(_ => new SqlDataAccess(new ConnectionSettings(config)))
    .BuildServiceProvider();

/// <summary>
/// Instantiating single connection
/// </summary>
var singleConnection = new SqlDataAccess(ConnectionStrings.Connections.FirstOrDefault());


/// <summary>
/// Instantiating multiple connections alongside some global settings such as connection timeout
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
string query = "SELECT name FROM sys.databases";
string queryFilter = "SELECT name FROM sys.databases Where name = @name";
var dbList1 = await db.LoadFromSqlAsync<string>(query);
var dbList1Filtered = await db.LoadFromSqlAsync<string, dynamic>(queryFilter, new {name = "master"});
var dbList2 = await singleConnection.LoadFromSqlAsync<string>(query);

// using other registered connections
var secondaryConnection = db.ConnectionSettings.GetConnectionStringByName("Secondary");
var dbList3 = await db.LoadFromSqlAsync<string>(query, secondaryConnection);
var dbList4 = await multipleConnections.LoadFromSqlAsync<string>(query, secondaryConnection);

await db.SaveFromSqlAsync("INSERT INTO Persons (Name) Values (@Name);", new[]
{
    new {Name = "John"}, new {Name = "Jane"}
});
#endregion
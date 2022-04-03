
[![Build status](https://dev.azure.com/vpetkovic/HelperTools/_apis/build/status/HelperTools/HelperTools)](https://dev.azure.com/vpetkovic/HelperTools/_build/latest?definitionId=3)
| CL.Sinks Package | NuGet | Version | Stats |
| --------------- | --------------- | --------------- | --------------- |
| SqlServer | [`Install-Package CL.Sinks.SqlServer`](https://www.nuget.org/packages/CL.Sinks.SqlServer/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.SqlServer) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.SqlServer?label=%20Downloads)
| MySql | [`Install-Package CL.Sinks.MySql`](https://www.nuget.org/packages/CL.Sinks.MySql/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.MySql) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.MySql?label=%20Downloads) |
| MySql Backup | [`Install-Package CL.Sinks.MySql.Backup`](https://www.nuget.org/packages/CL.Sinks.MySql.Backup/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.MySql.Backup) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.MySql.Backup?label=%20Downloads) |
| PostgreSql | [`Install-Package CL.Sinks.PostgreSql`](https://www.nuget.org/packages/CL.Sinks.PostgreSql/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.PostgreSql) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.PostgreSql?label=%20Downloads)

## Description
`CL.Sinks` is simple .NET library wrapped around Dapper ORM with a goal to simplify interactions with relational databases. 

## Background
The main motivation comes from unavoidable interaction with data sources, mainly ADO.NET providers, at work and for personal projects and dealing with repetative helper methods in each to handle connections and executing queries and stored procedures. 
Side note: I gave it a shot, but never seemed to adapt and get to like "code-first" approach with Entity Framework mainly because it is difficult to modify any table/field without having to constantly dealing with migrations back and forth. 

I needed very simple, fast and easy to setup data access layer for SqlServer, MySql and PostgreSql.

## Basic Usage

This basic example demonstrates how quickly you can connect and query your database whether it is SqlServer, MySql or PostgreSql. 
This is most appropriate approach for very simple projects or quick testing where as for more complex projects and scenarios that interact with multiple databases and database providers check out [more advanced usage](#advanced-and-production-ready-usage) examples
``` c#
// Connection strings will vary among providers. 
var connection = new Connection() { ConnectionString = "connection string" };

// Sql Server
var users = new SqlDataAccess(connection).LoadFromStoredProcedureAsync<Users>("sp_GetUsers");

// PostgreSql Server
var users = new PostgreSqlDataAccess(connection).LoadFromStoredProcedureAsync<Users>("sp_GetUsers");

// MySql
var users = new MySqlDataAccess(connection).LoadFromStoredProcedureAsync<Users>("sp_GetUsers");
```
<hr>

## Advanced Usage

There are several ways you can define your connections to different databases and database types. Most of the time you will end up using `appsettings.json` for it as connection strings will most likely come from some secrets manager services. Any connection string with connection name of "Default" will be mapped and considered your default connection.

`appsettings.json`
``` json
{
  "ConnectionStrings": {
    "Default": "sql server connection string",
    "MySql": "connection string",
    "PostgreSql": "connection string"
  }
}
```

If however, that is not the case and you want to hard code connecton strings you can define the list of connections by using `Connection` object
``` c#
public struct ConnectionStrings
{
    public static List<Connection> Connections = new()
    {
        new Connection
        {
            Name= "Default",
            ConnectionString = "sql server connection string",
            IsDefault = true,
        },
        new Connection
        {
            Name= "MySql",
            ConnectionString = "connection string",
            ConnectionTimeout = 120,
        },
        new Connection
        {
            Name= "PostgreSql",
            ConnectionString = "connection string"
        }
    };
}
```

The chances are that you will be using `Dependency Injection` to register data access providers. Let's showcase scenario with single provider and multiple providers and how you'd register and inject them in your app

Register services in `program.cs` (.NET 6) or `startup.cs` (.NETCore 3.1 - .NET 5) would be the same for both scenarios - you'd just register each provider as singleton
``` c#

// If you have defined connections in appsettings.json 
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, SqlDataAccess>(_ => new SqlDataAccess(new ConnectionSettings(config)))
    .AddSingleton<ISqlDataAccess, PostgreSqlDataAccess>(_ => new PostgreSqlDataAccess(new ConnectionSettings(config)))
    .AddSingleton<ISqlDataAccess, MySqlDataAccess>(_ => new MySqlDataAccess(new ConnectionSettings(config)
    {
        // You can specify default timeout for all connection strings. if not set it will default to 30 seconds
        GlobalConnectionTimeout = 90
    }))
    .BuildServiceProvider();
...
    
// If you have hard coded the definitions of connection strings in code behind
var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, SqlDataAccess>(_ => new SqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)))
    .AddSingleton<ISqlDataAccess, PostgreSqlDataAccess>(_ => new PostgreSqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)))
    .AddSingleton<ISqlDataAccess, MySqlDataAccess>(_ => new MySqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)))
    .BuildServiceProvider();
...   
```

#### Single Provider Usage
``` c#
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISqlDataAccess _sqlDataAccess;
    
    public UsersController(ISqlDataAccess dataProvider)
    {
        _sqlDataAccess = dataProvider;
    }

    [HttpGet("/sql/users")]
    public async Task<List<User>> SqlUsers() => await _sqlDataAccess.LoadFromStoredProcedureAsync<User>("sp_GetUsers");
    
    [HttpGet("/sql/users/{id:int}")]
    public async Task<User> SqlSingleUser() => await _sqlDataAccess.LoadFirstFromStoredProcedureAsync<User, dynamic>("sp_GetUsers", new {id});
}
```

#### Multiple Providers Usage
``` c#
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly ISqlDataAccess _mySqlDataAccess;
    private readonly ISqlDataAccess _postgreSqlDataAccess;
    
    public UsersController(IEnumerable<ISqlDataAccess> dataProviders)
    {
        _mySqlDataAccess = dataProviders.SingleOrDefault(s => s.GetType() == typeof(MySqlDataAccess));
        _postgreSqlDataAccess = dataProviders.SingleOrDefault(s => s.GetType() == typeof(PostgreSqlDataAccess));
        _sqlDataAccess = dataProviders.ElementAt(1);
    }

    [HttpGet("/sql/users")]
    public async Task<List<User>> SqlUsers() => await _sqlDataAccess.LoadFromStoredProcedureAsync<User>("sp_GetUsers");
    
    [HttpGet("/sql/users/{id:int}")]
    public async Task<User> SqlSingleUser() => await _sqlDataAccess.LoadFirstFromStoredProcedureAsync<User, dynamic>("sp_GetUsers", new {id});
    
    [HttpGet("/mysql/users")]
    public async Task<List<User>> MySqlUsers() => await _mySqlDataAccess.LoadFromStoredProcedureAsync<User>("sp_GetUsers", "MySql");
    
    [HttpGet("/postgresql/users")]
    public async Task<List<User>> PostgreSql() => await _postgreSqlDataAccess.LoadFromStoredProcedureAsync<User>("sp_GetUsers", "PostgreSql");
}
```

### All Methods

| Methods | Where | Returns | Notes |
| --------------- | --------------- | --------------- | --------------- |
| LoadFromSqlAsync<T, T1> | T required, T1 optional | List\<T\> |
| LoadFirstFromSqlAsync<T, T1> | T required, T1 optional | T | If query returns collection first item will be returned
| LoadFromStoredProcedureAsync<T, T1> | T required, T1 optional | List\<T\> |
| LoadFirstStoredProcedureAsync<T, T1> | T required, T1 optional | T | If sp returns collection first item will be returned
| SaveFromSqlAsync\<T\> | T optional | void |
| SaveFromStoredProcedureAsync\<T\> | T optional | void |
| SaveFromSql\<T\> | T optional | void |
| SaveFromStoredProcedure\<T\> | T optional | void |


<br>

## Extensions

### Fluent Database Backup Extension

Right now it is only supporting MySql backup but soon I will extend support for other database sinks and storage options ([choose priority](https://github.com/vpetkovic/CL.Sinks/discussions/8)) as well add restore functionality. 
*I am debating whether or not to combine this functionality into one extension or split them into multiple, one extension per sink. Feel free to drop your thoughts [here](https://github.com/vpetkovic/CL.Sinks/discussions/6)*

Anyways, in meantime if you use MySql database here's the sample usage

[`Install-Package CL.Sinks.MySql.Backup`](https://www.nuget.org/packages/CL.Sinks.MySql.Backup/)

``` c#

var backupStatus = FluentMySqlBackup
    .For(ConnectionStrings.Connections.FirstOrDefault(c => c.Name == "MySql"))
    .Export(new string[] { "all" }) // individial db names or "all" for all databases on server
    .ToLocalStorage(@"C:\users\vpetkovic\desktop")
        .Save() // multiThreaded = false
    .ToAzure("AzureBlobConnectionString")
        .OnBlob("backup-db-container")
        .Upload(true) // true = multiThreaded
    .Done();
```

`backupStatus` will return an object

``` json
{
    Exceptions: [],
    TotalBackupTimeMilliseconds: decimal,
    isSuccess: bool,
    IsRunning: bool,
}
```




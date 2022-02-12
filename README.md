*This library is dependent on Dapper as ORM but could be easily replaced or extended*

[![Build status](https://dev.azure.com/vpetkovic/HelperTools/_apis/build/status/HelperTools/HelperTools)](https://dev.azure.com/vpetkovic/HelperTools/_build/latest?definitionId=3)
| CL.Sinks Package | NuGet | Version | Stats |
| --------------- | --------------- | --------------- | --------------- |
| SqlServer | [`Install-Package CL.Sinks.SqlServer`](https://www.nuget.org/packages/CL.Sinks.SqlServer/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.SqlServer) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.SqlServer?label=%20Downloads)
| MySql | [`Install-Package CL.Sinks.MySql`](https://www.nuget.org/packages/CL.Sinks.MySql/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.MySql) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.MySql?label=%20Downloads) |
| MySql Backup | [`Install-Package CL.Sinks.MySql.Backup`](https://www.nuget.org/packages/CL.Sinks.MySql.Backup/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.MySql.Backup) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.MySql.Backup?label=%20Downloads) |
| PostgreSql | [`Install-Package CL.Sinks.PostgreSql`](https://www.nuget.org/packages/CL.Sinks.PostgreSql/) | ![Nuget](https://img.shields.io/nuget/v/CL.Sinks.PostgreSql) | ![Nuget](https://img.shields.io/nuget/dt/CL.Sinks.PostgreSql?label=%20Downloads)


## The Simplest Usage
This example demonstrates how simple and quick it is to connect and query your database. This is most useful approach for quick testing, for more production ready scenarios consider using [more advanced ways](#advanced-and-production-ready-usage)
``` c#
var usersList = new MySqlDataAccess(new Connection { ConnectionString = "connection string"})
    .LoadFromSqlAsync<UserModel, dynamic>(@"select * from users where isActive = @status", new { status = true });
```
<hr>

## Advanced and Production Ready Usage
### Using Dependency Injection and appsettings.json
This is the most common scenario for most of the apps nowadays and it is likely you will have the same database type but different connection strings (such as in microservices).

*Note: You can always mix and match different database types should your application requires that. (ex: Use PostgreSql and MySql and/or Sql Server)*

Example of how you'd access with multiple mysql databases in your application. 

`appsettings.json`
``` json
{
  "ConnectionStrings": {
    "Default": "connection string",
    "MySqlDb1": "connection string"
    "MySqlDb2": "connection string"
  }
}
```

`program.cs` (.NET 6) or `startup.cs` (.NETCore 3.1 - .NET 5)
``` c#
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .Build();

var serviceProvider = new ServiceCollection()
    .AddSingleton<ISqlDataAccess, MySqlDataAccess>(_ => new MySqlDataAccess(new ConnectionSettings(config)
    {
        // You can specify default timeout for all connection strings. if not set it will default to 30 seconds
        GlobalConnectionTimeout = 90
    }))
    .BuildServiceProvider();
    
var db = serviceProvider.GetRequiredService<ISqlDataAccess>();    
```

**Usage**
*Somewhere in your code where you need to get/post data to db*

``` c#
// It will use Default connection string
var authorFullName = "John Doe";
var books = await db.LoadFromSqlAsync<Books, dynamic>(@"select * from books where author = @authorFullName", new { authorFullName });

// Using other registered connection
var db1Conn = db.ConnectionSettings.GetConnectionStringByName("MySqlDb1");
db.SaveFromSql("delete from books where bookId = @id", new { id = 420 }, db1Conn);
db.SaveFromSql("delete from books where bookId = @id", new { id = 69 }, db1Conn);

var books = await db.LoadFromStoredProcedureAsync<string, dynamic>(@"sp_GetBooks @type", new { type = "mystery" }, "MySqlDb2");
```

### Not using Dependency Injection or appsettings.json 

``` c#
public struct ConnectionStrings
{
    public static List<Connection> Connections = new()
    {
        new Connection
        {
            Name= "MySqlDb1",
            ConnectionString = "connection string",
            IsDefault = true,
        },
        new Connection
        {
            Name= "MySqlDb2",
            ConnectionString = "connection string",
            ConnectionTimeout = 120,
        }
    };
}
```

``` c#
var singleConnection = new MySqlDataAccess(ConnectionStrings.Connections.FirstOrDefault(c => c.Name == "MySqlDb2"));
var multipleConnections = new MySqlDataAccess(new ConnectionSettings(ConnectionStrings.Connections)
```

**Usage**
*Somewhere in your code where you need to get/post data to db*

``` c#
// It will use MySqlDb2 connection string
var collection = await singleConnection.LoadFromSqlAsync<string, dynamic>("show databases", new {}); 

// Using other registered connection 
var db2Conn = db.ConnectionSettings.GetConnectionStringByName("MySqlDb2");
var collection = await multipleConnections.LoadFromSqlAsync<string, dynamic>("show databases", new {}, db2Conn);
```

## Extensions

### Fluent Database Backup Extension

Right now it is only supporting MySql backup but soon I will extend support for other database sinks and storage options ([choose priority](https://github.com/vpetkovic/CL.Sinks/discussions/8)) as well add restore functionality. 
*I am debating whether or not to combine this functionality into one extension or split them into multiple, one extension per sink. Feel free to drop your thoughts [here](https://github.com/vpetkovic/CL.Sinks/discussions/6)*

Anyways, in meantime if you use MySql database here's the sample usage

[`Install-Package CL.Sinks.MySql.Backup`](https://www.nuget.org/packages/CL.Sinks.MySql.Backup/)

``` c#

var backupStatus = FluentMySqlBackup
    .For(ConnectionStrings.Connections.FirstOrDefault(c => c.Name == "MySqlDb2"))
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




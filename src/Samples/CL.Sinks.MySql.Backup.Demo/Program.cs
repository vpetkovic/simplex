using CL.Sinks.MySql;
using CL.Sinks.MySql.Backup;
using CL.Sinks.MySql.Backup.Demo;
using Microsoft.Extensions.Configuration;

// Leveraging CL.Sinks.MySql but could be any other data mysql provider
var singleConnection = new MySqlDataAccess(ConnectionStrings.Connections.FirstOrDefault()).ConnectionSettings.DefaultConnection;

var backupStatus = FluentMySqlBackup
    .For(singleConnection)
    .Export(new string[] { "all" }) // individial db names or "all" for all databases on server
    .ToLocalStorage(@"C:\users\vpetkovic\desktop")
        .Save()
    .ToAzure("AzureBlobConnectionString")
        .OnBlob("test")
        .Upload(true)
    .Done();

Console.ReadLine();

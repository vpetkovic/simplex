using CL.Sinks.MySql.Backup;
using CL.Sinks.MySql.Backup.Demo;

var backupStatus = FluentMySqlBackup
    .For(ConnectionStrings.Connections.FirstOrDefault())
    .Export(new string[] { "all" }) // individial db names or "all" for all databases on server
    .ToLocalStorage(@"C:\users\vpetkovic\desktop")
        .Save()
    .ToAzure("AzureBlobConnectionString")
        .OnBlob("test")
        .Upload(true)
    .Done();

Console.ReadLine();

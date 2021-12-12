namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    public interface IAzureAction
    {
        IMySqlServerActions Upload(bool multiThreaded = false);
    }


}

namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    public interface ILocalStorage
    {
        IMySqlServerActions Save(bool multiThreaded = false);
    }


}

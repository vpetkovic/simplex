namespace CL.Sinks.MySql.Backup
{
    public interface ILocalStorage
    {
        IMySqlServerActions Save(bool multiThreaded = false);
    }


}

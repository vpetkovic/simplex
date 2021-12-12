namespace CL.Sinks.MySql.Backup
{
    public interface IAzureAction
    {
        IMySqlServerActions Upload(bool multiThreaded = false);
    }


}

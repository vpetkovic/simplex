namespace CL.Sinks.MySql.Backup.MySqlConnector
{
    public interface IAzureBlob
    {
        /// <exception cref="ArgumentException"></exception>
        IAzureAction OnBlob(string blobBlockPath);
    }


}

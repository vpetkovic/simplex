namespace CL.Sinks.MySql.Backup
{
    public interface IAzureBlob
    {
        /// <exception cref="ArgumentException"></exception>
        IAzureAction OnBlob(string blobBlockPath);
    }


}

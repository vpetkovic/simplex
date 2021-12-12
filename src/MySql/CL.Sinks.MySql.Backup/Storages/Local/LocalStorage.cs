namespace CL.Sinks.MySql.Backup
{
    /// <summary>
    /// Local Storage
    /// </summary>
    public sealed partial class FluentMySqlBackup : ILocalStorage
    {
        public IMySqlServerActions Save(bool multiThreaded = false) { ExecuteFor(_localStorageModel, multiThreaded); return this; }
    }
}

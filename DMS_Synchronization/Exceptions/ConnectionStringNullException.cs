namespace DMS_Synchronization.Exceptions
{
    using System;

    public partial class ConnectionStringNullException : Exception
    {
        public ConnectionStringNullException(string connectionName) : base(connectionName + " connectionString is null.")
        {

        }
    }
}

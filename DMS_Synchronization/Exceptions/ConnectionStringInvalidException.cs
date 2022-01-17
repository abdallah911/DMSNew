namespace DMS_Synchronization.Exceptions
{
    using System;

    public partial class ConnectionStringInvalidException : Exception
    {
        public ConnectionStringInvalidException() : base("connectionString is invalid.")
        {

        }
    }
}

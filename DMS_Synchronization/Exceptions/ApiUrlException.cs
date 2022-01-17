namespace DMS_Synchronization.Exceptions
{
    using System;

    public partial class ApiUrlException : Exception
    {
        public ApiUrlException(string message, Exception ex) : base("cann't open a connection with api url: " + message + ", it is invalid url or the api is not online.", ex)
        {

        }
    }
}

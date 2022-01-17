using System;


namespace DMS_Synchronization.ViewModels
{
    public class Error
    {
        public string Table { get; set; }
        public string Database { get; set; }
        public string Server { get; set; }
        public string Action { get; set; }
        public Exception Exception { get; set; }
    }
}

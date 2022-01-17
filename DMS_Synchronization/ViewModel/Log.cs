

namespace DMS_Synchronization.ViewModels
{
    public class Log
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Table { get; set; }
        public string Action { get; set; }
        public string Note { get; set; } = "";
        public long? AffectedRows { get; set; }
        public long Order { get; set; }
    }
}

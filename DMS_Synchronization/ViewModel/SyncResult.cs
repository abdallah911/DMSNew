
using System.Collections.Generic;


namespace DMS_Synchronization.ViewModels
{
    public class SyncResult
    {
        public List<Log> Logs { get; set; } = new List<Log>();
        public List<Error> Errors { get; set; } = new List<Error>();
        public bool Succeeded { get; set; }
        public bool SucceededWithErrors { get; set; }
    }
}

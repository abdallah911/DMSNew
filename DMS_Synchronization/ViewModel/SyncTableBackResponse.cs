
using System.Collections.Generic;

namespace DMS_Synchronization.ViewModels
{
    internal class SyncTableBackResponse<Tse, Tback>
    {
        public List<Tback> BackOfficeData { get; set; } = new List<Tback>();
        public List<Tse> SEData { get; set; } = new List<Tse>();
    }
}


using System;

namespace DMS_Synchronization.ViewModel
{
    public class MEDICINE_GRUOP
    {
        public int GRUOP_ID { get; set; }
        public string GRUOP_NAME { get; set; }
        public string GRUOP_TYPE { get; set; }
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
    }
}
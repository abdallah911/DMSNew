using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.ViewModel
{
    public class ADMINDIAGNOSIS
    {
        public string ORACLEID { get; set; }
        public string SPECIALIST { get; set; }
        public string DIAGNOSIS { get; set; }
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public string CREATEDBY { get; set; }
        public string UPDATEDBY { get; set; }
        public DateTime? UPDATEDDATE { get; set; }
    }
}
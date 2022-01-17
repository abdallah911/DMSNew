using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.BaseEntity
{
    public class BaseEntityOraDB
    {
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
    }
}
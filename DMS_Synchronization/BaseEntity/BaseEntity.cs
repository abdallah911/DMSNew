using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.BaseEntity
{
    public class BaseEntityDB
    {
        public bool IsSync { get; set; }
        public DateTime? SyncDate { get; set; }
        public string SyncBy { get; set; }
    }
}
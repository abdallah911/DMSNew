using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class PollDataChronic : BaseEntityDB
    {
        public decimal POLL_CODE { get; set; }
        public string ACTIVE { get; set; }
        public decimal CHRONIC_CODE { get; set; }
    }
}
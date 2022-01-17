using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class PollDataDiag : BaseEntityDB
    {
        public decimal POLL_CODE { get; set; }
        public string ACTIVE { get; set; }
        public decimal DIA_CODE { get; set; }
    }
}
using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class CONSUMPTION_POOL : BaseEntityDB
    {
        public int COMP_ID { get; set; }
        public int CONTRACT_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public int COST_CODE { get; set; }
        public int POLL_CODE { get; set; }
        public string NAME_POOL { get; set; }
        public double MAX_AMOUNT { get; set; }
        public double AMOUNT { get; set; }
        public decimal REMAINING { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public DateTime LAST_UPDATED_DATE { get; set; }
      
    }
}
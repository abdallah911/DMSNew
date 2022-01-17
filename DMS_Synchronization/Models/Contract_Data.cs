using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class Contract_Data:BaseEntityDB
    {
        public Decimal C_COMP_ID { get; set; }
        public Decimal CONTRACT_NO { get; set; }
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public string PROPOSAL_CODE { get; set; }
        public DateTime? DATE_FROM { get; set; }
        public DateTime? DATE_TO { get; set; }
        public string DONE_FLAG { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
    }
}
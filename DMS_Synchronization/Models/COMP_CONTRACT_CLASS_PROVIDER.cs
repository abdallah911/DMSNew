using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class COMP_CONTRACT_CLASS_PROVIDER : BaseEntityDB
    {
        public decimal COMP_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public decimal CONTRACT_NO { get; set; }
        public decimal PROV_DEGREE { get; set; }
        public decimal PRV_TYP { get; set; }
        public decimal PR_CODE { get; set; }
        public decimal SERV_CODE { get; set; }
        public decimal COPAY_AMT { get; set; }
        public decimal COPAY_PERC { get; set; }
        public decimal MAX_AMOUNT { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string TYPE { get; set; }
        public string PR_NAME { get; set; }
        public string RESIDENT_DEGREE { get; set; }

    }
}
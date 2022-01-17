using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public partial class PollData : BaseEntityDB
    {
        public decimal POLL_CODE { get; set; }
        public decimal COMP_ID { get; set; }
        public decimal CONTRACT_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public decimal? COST_CODE { get; set; }
        public decimal? RELATION_CODE { get; set; }
        public decimal? FROM_AGE { get; set; }
        public decimal? FAGE_CODE { get; set; }
        public decimal? TO_AGE { get; set; }
        public decimal? TAGE_CODE { get; set; }
        public string CARD_ID { get; set; }
        public decimal? AMOUNT { get; set; }
        public decimal? AMOUNT_FROM { get; set; }
        public decimal? AMOUNT_TYPE { get; set; }
        public decimal? PERCENT_TYPE { get; set; }
        public decimal? COUNT_EMP { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public decimal? NUMBER_EMP { get; set; }
        public string NAME_POOL { get; set; }
        public decimal? SUB_CODE { get; set; }
        public string ALL_CARDS { get; set; }
        public decimal? APPLY_TO { get; set; }
    }
}
using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class COMP_CUSTOMIZED_D_D_MED : BaseEntityDB
    {
        public decimal COMP_ID { get; set; }
        public decimal BRANCH_CODE { get; set; }
        public decimal C_COMP_ID { get; set; }
        public decimal CONTRACT_NO { get; set; }
        public string SERV_CODE { get; set; }
        public string D_SERV_CODE { get; set; }
        public string CLASS_CODE { get; set; }
        public string SER_SERV { get; set; }
        public decimal? DAY_AMT { get; set; }
        public decimal? DAY_NO { get; set; }
        public decimal? MON_AMT { get; set; }
        public decimal? MON_NO { get; set; }
        public decimal? LAB_NO { get; set; }
        public decimal? RAY_NO { get; set; }
        public decimal? LAB_NO_MON { get; set; }
        public decimal? RAY_NO_MON { get; set; }
        public decimal? DAY_MED_AMT_MON { get; set; }
        public decimal? DAY_NO_ROSHTA_MON { get; set; }
        public decimal? MON_MED_AMT_MON { get; set; }
        public decimal? MON_NO_ROSHTA_MON { get; set; }
        public decimal? DAY_MED_AMT_YEAR { get; set; }
        public decimal? DAY_NO_ROSHTA_YEAR { get; set; }
        public decimal? MON_MED_AMT_YEAR { get; set; }
        public decimal? MON_NO_ROSHTA_YEAR { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public decimal? VISIT_NO { get; set; }
        public decimal? VISIT_NO_MON { get; set; }
        public decimal? SESSION_NO { get; set; }
        public decimal? SESSION_NO_MON { get; set; }
    }
}
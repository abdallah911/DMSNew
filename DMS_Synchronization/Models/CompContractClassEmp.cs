using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class CompContractClassEmp : BaseEntityDB
    {
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public Decimal C_COMP_ID { get; set; }
        public Decimal CONTRACT_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public string CARD_ID { get; set; }
        public Decimal EMP_CODE { get; set; }
        public Decimal AREA_CODE { get; set; }
        public Decimal MAX_AMOUNT { get; set; }
        public Decimal ANNUAL_PREM { get; set; }
        public Decimal HOSPITAL_DEGREE { get; set; }
        public Decimal COVER_RELATION { get; set; }
        public Decimal OVER_AGE_COND { get; set; }
        public Decimal OVER_AGE_PERT { get; set; }
        public Decimal OVER_AGE_AMT { get; set; }
        public Decimal AMBULANCE { get; set; }
        public Decimal AMBULANCE_AMT { get; set; }
        public Decimal NO_EMPLOYEES { get; set; }
        public Decimal MAX_CEILING { get; set; }
        public string CRIT_CASE { get; set; }
        public Decimal CRIT_CASE_DAYS { get; set; }
        public Decimal CRIT_CASE_AMT { get; set; }
        public Decimal PERT_CEILING { get; set; }
        public Decimal COVER_RELATION_AMT { get; set; }
        public Decimal SON_OVER_COVER_DATE { get; set; }
        public Decimal CARD_FEE_FRST { get; set; }
        public Decimal CARD_FEE_OTH { get; set; }
        public Decimal CASH_CARD_FLAG { get; set; }
        public Decimal COVER_AGE_FROM { get; set; }
        public Decimal COVER_AGE_TO { get; set; }
        public Decimal SON_COVER_FROM { get; set; }
        public Decimal SON_COVER_TO { get; set; }
        public Decimal ALLOW_PERIOD { get; set; }
        public Decimal INTEREST_TYP { get; set; }
        public Decimal INTEREST_VAL { get; set; }
        public Decimal DELAY_PERT { get; set; }
        public Decimal STOP_INDEM { get; set; }
        public Decimal STOP_APPROVAL { get; set; }
        public Decimal STOP_ISSUE_CARD { get; set; }
        public Decimal DELIVER_CHECKS_PERIOD { get; set; }
        public Decimal P_BEF_CANCEL_CONTRACT { get; set; }
        public Decimal TRAVEL_COVER_AMT { get; set; }
        public Decimal TRAVEL_COVER_PERT { get; set; }
        public DateTime? START_COVER { get; set; }
        public DateTime? END_COVER { get; set; }
        public Decimal SON_RELATION_TYP { get; set; }
        public Decimal OVER_AGE_METHOD { get; set; }
        public Decimal OVER_CEILING_PERT { get; set; }
        public string OVER_CEILING_PROC { get; set; }
        public string PRINT_CARD { get; set; }
        public Decimal PRINTED_NUM { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public DateTime? PRINT_DATE { get; set; }
        public DateTime? RE_PRINT_DATE { get; set; }
        public DateTime? STOP_DATE { get; set; }
        public string DLEV_CARD { get; set; }
        public DateTime? DELV_DATE { get; set; }
        public string FOR_FAMILY { get; set; }

    }
}
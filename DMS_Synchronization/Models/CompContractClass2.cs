using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class CompContractClass2:BaseEntityDB
    {
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public Decimal C_COMP_ID { get; set; }
        public Decimal CONTRACT_NO { get; set; }
        public string CLASS_CODE { get; set; }
        public Decimal AREA_CODE { get; set; }
        public string FILE_IMG { get; set; }
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
        public Decimal DecimalEREST_VAL { get; set; }
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
        public string CHK_RECOLLECTION { get; set; }
        public Decimal OVER_CEILING_PERT { get; set; }
        public string OVER_CEILING_PROC { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string DEPUTY_MAN { get; set; }
        public string MATERNITY_COVER { get; set; }
        public Decimal MATERNITY_DURATION { get; set; }
        public string PRDecimal_CARD_TYP { get; set; }
        public Decimal INDMENITY_PERIOD { get; set; }
        public Decimal PRICE_LIST { get; set; }
        public Decimal PRICE_MAX_AMT { get; set; }
        public Decimal PRICE_MAX_PRCT { get; set; }
        public string AGE_FROM_TYP { get; set; }
        public string AGE_TO_TYP { get; set; }
        public string AGE_FROM_SON_TYP { get; set; }
        public string AGE_TO_SON_TYP { get; set; }
        public Decimal COVER_TYP { get; set; }
        public Decimal MONTHLY_MED_COVER { get; set; }
        public string ONLINE_NOTES { get; set; }

    }
}
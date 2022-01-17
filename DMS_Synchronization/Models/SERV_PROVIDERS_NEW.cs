using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class SERV_PROVIDERS_NEW : BaseEntityDB
    {
        public Decimal PR_CODE { get; set; }
        public Decimal COMP_ID { get; set; }
        public Decimal BRANCH_CODE { get; set; }
        public Decimal PRV_TYPE { get; set; }
        public string PR_ANAME { get; set; }
        public string PR_ENAME { get; set; }
        public Decimal OLD_PR_CODE { get; set; }
        public string DOC_SPEC { get; set; }
        public string PR_DESC { get; set; }
        public string ADDRESS1 { get; set; }
        public string ADDRESS2 { get; set; }
        public string TEL1 { get; set; }
        public string TEL2 { get; set; }
        public string FAX { get; set; }
        public string EMAIL { get; set; }
        public Decimal AREA_CODE { get; set; }
        public string PERSON1 { get; set; }
        public string PERSON2 { get; set; }
        public string PERSON1_TEL { get; set; }
        public string PERSON2_TEL { get; set; }
        public string INSIDE_NWT { get; set; }
        public string OUTSIDE_NWT { get; set; }
        public string TERMINATE_FLAG { get; set; }
        public DateTime? CONTRACT_DATE { get; set; }
        public DateTime? TERMINATE_DATE { get; set; }
        public Decimal PR_TYPE { get; set; }
        public Decimal PR_AREA_COVER { get; set; }
        public DateTime? TAX_AVOID_DATE { get; set; }
        public string TAX_CARD { get; set; }
        public string TAX_FILE { get; set; }
        public Decimal PAY_PER_MONTH { get; set; }
        public Decimal TAX_FLG { get; set; }
        public Decimal SERV_FLG { get; set; }
        public Decimal STAMP_VALUE { get; set; }
        public string TAX_ROOM { get; set; }
        public Decimal SUB_PRV_TYPE { get; set; }
        public Decimal ISSUE_PER_YEAR { get; set; }
        public Decimal PR_TAX1 { get; set; }
        public Decimal PR_TAX2 { get; set; }
        public string PROV_DEGREE { get; set; }
        public Decimal DEV_CHK { get; set; }
        public Decimal P_LIST { get; set; }
        public Decimal PRICE_DISC { get; set; }
        public Decimal REORDER_LEVEL { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string GL_ACC_NO { get; set; }
        public string ACC_NO_DED_TAX { get; set; }
        public Decimal MAX_APPROVAL_NO { get; set; }
        public Decimal MAX_APPROVAL_AMT { get; set; }
        public Decimal FOR_MED_DIS { get; set; }
        public Decimal LOCAL_MED_DIS { get; set; }
        public string MAIN_SERV { get; set; }
        public Decimal DEV_STAND { get; set; }
        public Decimal DEV_LOC { get; set; }
        public Decimal DEV_EXT { get; set; }
        public Decimal GOVERNMENT_CODE { get; set; }
        public string ACTIVATION_BY { get; set; }
        public DateTime? ACTIVATION_DATE { get; set; }
        public string DEACTIVATION_BY { get; set; }
        public DateTime? DEACTIVATION_DATE { get; set; }
        public string REASON_DEACTIVATION { get; set; }
        public string REASON_ACTIVATION { get; set; }
        public string MAIN_CODE { get; set; }
    }
}
namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comp_Employees: BaseEntityDB
    {
       // public Decimal Id { get; set; }

       
        public string CARD_ID { get; set; }

        public decimal? COMP_ID { get; set; }

        public decimal? BRANCH_CODE { get; set; }

        public decimal? C_COMP_ID { get; set; }

        public decimal? CONTRACT_NO { get; set; }
        
        public string CLASS_CODE { get; set; }

        public decimal? DEPT_ID { get; set; }

        public decimal? EMP_CODE { get; set; }

        public string EMP_ANAME { get; set; }

        public string EMP_ENAME { get; set; }

        public string ADDRESS1 { get; set; }

        public string ADDRESS2 { get; set; }

        public string TEL1 { get; set; }

        public string TEL2 { get; set; }

        public string FAX { get; set; }

        public string EMAIL { get; set; }

        public DateTime? birth_date { get; set; }

        public DateTime? HIRE_DATE { get; set; }

        public string INS_TYP { get; set; }

        public string REF_EMP { get; set; }

        public string MATERIAL_STATUS { get; set; }

        public string TERMINATE_FLAG { get; set; }

        public DateTime? TERMINATE_DATE { get; set; }

        public decimal? EXP_CELLING { get; set; }

        public decimal? GENDER { get; set; }

        public DateTime? INS_START_DATE { get; set; }

        public DateTime? INS_END_DATE { get; set; }

        public string W_GLASS { get; set; }

        public decimal? EXP_AGE { get; set; }

        public string EMP_ID { get; set; }

        public string EMP_INSURANCE_NO { get; set; }

        public string EMP_IMG { get; set; }

        public string OLD_CLASS_CODE { get; set; }

        public string TRANS_EMO_TYPE { get; set; }

        public string EFFECT_DATE_TYPE { get; set; }

        public DateTime? SPECIFIC_DATE { get; set; }

        public DateTime? DELV_CARD_DATE { get; set; }

        public string PRINT_CARD { get; set; }

        public string NOTES { get; set; }

        public string ACTIVE { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        public string CREATED_BY { get; set; }

        public string UPDATE_BY { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        public string EMP_ANAME_ST { get; set; }

        public string EMP_ANAME_SC { get; set; }

        public string EMP_ANAME_TH { get; set; }

        public string EMP_ANAME_FR { get; set; }

        public string EMP_ANAME_FAM { get; set; }

        public string EMP_ENAME_ST { get; set; }

        public string EMP_ENAME_SC { get; set; }

        public string EMP_ENAME_TH { get; set; }

        public string EMP_ENAME_FR { get; set; }

        public string EMP_ENAME_FAM { get; set; }

        public string DEL_DELIVIER_CARD { get; set; }

        public decimal? BK_CODE { get; set; }

        public string BK_ACC_NO { get; set; }

        public string COST_CODE { get; set; }

        public string PRINT_FST_NAME { get; set; }

        public string PRINT_SEC_NAME { get; set; }

        public string PRINT_THR_NAME { get; set; }

        public string PRINT_FTH_NAME { get; set; }

        public string PRINT_LST_NAME { get; set; }

        public string PRINT_TYP { get; set; }

        public DateTime? END_WRK_DT { get; set; }

        public string USR_TYP { get; set; }

        public decimal? EMP_SEQ { get; set; }

    }
}

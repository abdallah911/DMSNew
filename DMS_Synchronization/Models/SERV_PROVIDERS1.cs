namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Serv_Providers1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PR_CODE { get; set; }

        public int COMP_ID { get; set; }

        public int BRANCH_CODE { get; set; }

        public int PRV_TYPE { get; set; }

        [StringLength(100)]
        public string PR_ANAME { get; set; }

        [StringLength(100)]
        public string PR_ENAME { get; set; }

        public int? OLD_PR_CODE { get; set; }

        [StringLength(100)]
        public string DOC_SPEC { get; set; }

        [StringLength(100)]
        public string PR_DESC { get; set; }

        [StringLength(100)]
        public string ADDRESS1 { get; set; }

        [StringLength(100)]
        public string ADDRESS2 { get; set; }

        [StringLength(50)]
        public string TEL1 { get; set; }

        [StringLength(50)]
        public string TEL2 { get; set; }

        [StringLength(50)]
        public string FAX { get; set; }

        [StringLength(50)]
        public string EMAIL { get; set; }

        public int? AREA_CODE { get; set; }

        [StringLength(50)]
        public string PERSON1 { get; set; }

        [StringLength(50)]
        public string PERSON2 { get; set; }

        [StringLength(50)]
        public string PERSON1_TEL { get; set; }

        [StringLength(50)]
        public string PERSON2_TEL { get; set; }

        [StringLength(1)]
        public string INSIDE_NWT { get; set; }

        [StringLength(1)]
        public string OUTSIDE_NWT { get; set; }

        [StringLength(1)]
        public string TERMINATE_FLAG { get; set; }

        public DateTime? CONTRACT_DATE { get; set; }

        public DateTime? TERMINATE_DATE { get; set; }

        public int? PR_TYPE { get; set; }

        public int? PR_AREA_COVER { get; set; }

        public DateTime? TAX_AVOID_DATE { get; set; }

        [StringLength(30)]
        public string TAX_CARD { get; set; }

        [StringLength(30)]
        public string TAX_FILE { get; set; }

        public int? PAY_PER_MONTH { get; set; }

        public int? TAX_FLG { get; set; }

        public int? SERV_FLG { get; set; }

        public int? STAMP_VALUE { get; set; }

        [StringLength(30)]
        public string TAX_ROOM { get; set; }

        public int? SUB_PRV_TYPE { get; set; }

        public int? ISSUE_PER_YEAR { get; set; }

        public int? PR_TAX1 { get; set; }

        public int? PR_TAX2 { get; set; }

        [StringLength(100)]
        public string PROV_DEGREE { get; set; }

        public int? DEV_CHK { get; set; }

        public int? P_LIST { get; set; }

        public int? PRICE_DISC { get; set; }

        public int? REORDER_LEVEL { get; set; }

        [StringLength(1)]
        public string ACTIVE { get; set; }

        [StringLength(100)]
        public string CREATED_BY { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        [StringLength(100)]
        public string UPDATE_BY { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        [StringLength(100)]
        public string GL_ACC_NO { get; set; }

        [StringLength(100)]
        public string ACC_NO_DED_TAX { get; set; }

        public int? MAX_APPROVAL_NO { get; set; }

        public int? MAX_APPROVAL_AMT { get; set; }

        public int? FOR_MED_DIS { get; set; }

        public int? LOCAL_MED_DIS { get; set; }

        [StringLength(100)]
        public string MAIN_SERV { get; set; }

        public int? DEV_STAND { get; set; }

        public int? DEV_LOC { get; set; }

        public int? DEV_EXT { get; set; }
    }
}

namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Basic_Data:BaseEntityDB
    {
       // public int Id { get; set; }

        public decimal BS_CODE { get; set; }

        [Required]
        [StringLength(100)]
        public string SOURCE_MOD { get; set; }

        public decimal COMP_ID { get; set; }

        public decimal BRANCH_CODE { get; set; }

        [StringLength(100)]
        public string BS_CODE_UP { get; set; }

        [StringLength(300)]
        public string BS_ANAME { get; set; }

        [StringLength(300)]
        public string BS_ENAME { get; set; }

        [StringLength(10)]
        public string BS_TYPE { get; set; }

        [StringLength(100)]
        public string NOTES { get; set; }

        [StringLength(1)]
        public string ACTIVE { get; set; }

        [StringLength(100)]
        public string SERV_CODE { get; set; }

        [StringLength(50)]
        public string PHONE { get; set; }

        [StringLength(50)]
        public string EMAIL { get; set; }

        public decimal? AMT { get; set; }

        public DateTime? START_DATE { get; set; }

        public DateTime? END_DATE { get; set; }

        public DateTime? CREATED_DATE { get; set; }

        [StringLength(100)]
        public string CREATED_BY { get; set; }

        [StringLength(100)]
        public string UPDATE_BY { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        public decimal? ISSUE_QTY { get; set; }

        public decimal? DELV_QTY { get; set; }

        [StringLength(1)]
        public string ADJUST_TYP { get; set; }

        public decimal? ADJUST_PERIOD { get; set; }

        public decimal? MAX_COLLECT_TIME { get; set; }

        [StringLength(1)]
        public string USED_TYP { get; set; }

        public decimal? USED_VAL { get; set; }

        public decimal? USED_PERIOD { get; set; }

        public decimal? NO_RETURN_MONTH { get; set; }

        [StringLength(1)]
        public string USED_TYP_ADD { get; set; }
    }
}

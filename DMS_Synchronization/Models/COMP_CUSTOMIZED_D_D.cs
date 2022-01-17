namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comp_Customized_D_D:BaseEntityDB
    {
       // public int ID { get; set; }

        public decimal? COMP_ID { get; set; }

        public decimal? BRANCH_CODE { get; set; }

        public decimal? C_COMP_ID { get; set; }

        public decimal? CONTRACT_NO { get; set; }

        [StringLength(100)]
        public string SERV_CODE { get; set; }

        [StringLength(100)]
        public string D_SERV_CODE { get; set; }

        [StringLength(10)]
        public string CLASS_CODE { get; set; }

        [StringLength(100)]
        public string SER_SERV { get; set; }

        public decimal? CEILING_AMT { get; set; }

        public decimal? CEILING_PERT { get; set; }

        public decimal? NO_DAYS { get; set; }

        [StringLength(100)]
        public string NOTES { get; set; }

        [StringLength(1)]
        public string ACTIVE { get; set; }

        [StringLength(100)]
        public string CREATED_BY { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CREATED_DATE { get; set; }

        [StringLength(100)]
        public string UPDATE_BY { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UPDATE_DATE { get; set; }

        [StringLength(100)]
        public string SER_SERV_H { get; set; }

        [StringLength(1)]
        public string POLL_FLAG { get; set; }

        [StringLength(1)]
        public string SERV_DET_LEV { get; set; }

        [StringLength(1)]
        public string REQ_APPRPV { get; set; }

        [StringLength(1)]
        public string REQ_LETTER { get; set; }

        [StringLength(10)]
        public string IND_LIST_PRICE { get; set; }

        public decimal? REPEAT_NO { get; set; }

        [StringLength(1)]
        public string REPEAT_TYP { get; set; }

        public decimal? REPEAT_PERIOD { get; set; }

        public decimal? CARR_AMT { get; set; }

        [StringLength(10)]
        public string MAT_COV_TYP { get; set; }

        [StringLength(10)]
        public string DOC_EXP { get; set; }

        [StringLength(10)]
        public string DOC_EXP_VAL_TYP { get; set; }

        public decimal? DOC_EXP_VALUE { get; set; }

        [StringLength(1)]
        public string REFUND_FLAG { get; set; }

    }
}

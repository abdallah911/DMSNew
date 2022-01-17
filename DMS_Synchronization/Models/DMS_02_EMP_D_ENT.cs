namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Dms_02_Emp_D_Ent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int D_SEQ { get; set; }

        public long D_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime D_DATE { get; set; }

        public int D_VD { get; set; }

        public int DD_NUM { get; set; }

        public int? D_VC { get; set; }

        public int DC_NUM { get; set; }

        public int? PERCENT_MONY { get; set; }

        public string CARD_ID { get; set; }

        public string D_EXP { get; set; }

        public string D_EXP_2 { get; set; }

        public string D_EXP_3 { get; set; }

        public string D_EXP_4 { get; set; }

        public string CUST_E_NAME { get; set; }

        public string CUST_E_NAME_2 { get; set; }

        public string CUST_E_NAME_3 { get; set; }

        public string CUST_E_NAME_4 { get; set; }

        public int? D_TAX { get; set; }

        public string MANAGER { get; set; }

        public string INVOICE { get; set; }

        public string NOTES_INV { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DATE_INVOICE { get; set; }

        public int? EMP_ID_ID { get; set; }

        public string EMP_SUB { get; set; }

        public long? EMP_ID { get; set; }

        [StringLength(100)]
        public string EMP_NAME { get; set; }

        public int? MGR_ID { get; set; }

        public int C_COMP_ID { get; set; }

        [StringLength(250)]
        public string REGUL_1 { get; set; }

        [StringLength(10)]
        public string CLASS_CODE { get; set; }

        [Column(TypeName = "date")]
        public DateTime? INS_START_DATE { get; set; }

        [Column(TypeName = "date")]
        public DateTime? INS_END_DATE { get; set; }

        [StringLength(250)]
        public string TAFK_01 { get; set; }

        [StringLength(150)]
        public string C_COMP_NAME { get; set; }

        public int? INSU_LIMT { get; set; }

        public int? INSU_LIMT_M { get; set; }

        public int? CARRY { get; set; }

        public int? NON_COVERED { get; set; }

        public int? VALUE_CREDIT { get; set; }

        public int? OVER_INSURANCE { get; set; }

        public int? VALUE_CASH { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DATE_ROSHTA { get; set; }

        [StringLength(150)]
        public string TAKHASOS { get; set; }

        public int? OVER_OVER { get; set; }

        [StringLength(150)]
        public string OVER_NAME { get; set; }

        public int? OVER_KIND { get; set; }

        [StringLength(25)]
        public string D_TIME { get; set; }

        [StringLength(150)]
        public string TASHKHES_01 { get; set; }

        [StringLength(150)]
        public string TASHKHES_02 { get; set; }

        [StringLength(150)]
        public string TASHKHES_03 { get; set; }

        public int? OVER_OVER_01 { get; set; }

        [StringLength(150)]
        public string OVER_NAME_01 { get; set; }

        public int? OVER_KIND_01 { get; set; }

        public int? DALY_SEQ { get; set; }

        public int? REGS_NO { get; set; }

        public int? DOC_ID { get; set; }

        [StringLength(120)]
        public string DOC_NAME { get; set; }

        public int? TYPE_DOC { get; set; }

        public int? TYPE_VISIT { get; set; }

        public int? APPROV_CLAIM { get; set; }

        public int? APPROV_PAY { get; set; }

        [StringLength(25)]
        public string REV_DATE { get; set; }

        [StringLength(25)]
        public string REV_TIME { get; set; }

        public int? BATSH_AMT { get; set; }

        public int? BATSH_NO { get; set; }

        public int? REVIEW_ID { get; set; }

        [StringLength(150)]
        public string REVIEW_NAME { get; set; }

        public int? KIND_KIND { get; set; }

        public int? ID_DOC { get; set; }

        [StringLength(125)]
        public string N_DOC { get; set; }

        public int? HOLD_CLAM { get; set; }

        [StringLength(25)]
        public string RECIT { get; set; }

        public long? BATSH_REP { get; set; }

        public int? MASS_NO { get; set; }

        public int? DISC { get; set; }

        public int? TOT_DISC { get; set; }

        public int? GROUP_ID { get; set; }

        public int? MONY_ACT { get; set; }

        public int? PER_LOC { get; set; }

        public int? PER_IMP { get; set; }

        public int? LOC_AMOUNT { get; set; }

        public int? IMP_AMOUNT { get; set; }

        public int? KIND_REP { get; set; }

        public int? TN1 { get; set; }

        public int? TN2 { get; set; }

        public int? DEV_LOC { get; set; }

        public int? DEV_IMP { get; set; }

        public int? APRROV_COUN { get; set; }

        [StringLength(50)]
        public string DISC_NAME { get; set; }

        public int? P_1 { get; set; }

        public int? P_2 { get; set; }

        public int? P_3 { get; set; }

        public int? P_4 { get; set; }

        public int? P_5 { get; set; }

        public int? P_6 { get; set; }

        public int? P_7 { get; set; }

        public int? P_8 { get; set; }

        public int? P_9 { get; set; }

        public int? P_10 { get; set; }

        public int? P_11 { get; set; }

        public int? P_12 { get; set; }

        public int? P_13 { get; set; }

        public int? P_14 { get; set; }

        public int? P_15 { get; set; }

        public int? P_16 { get; set; }

        public int? P_17 { get; set; }

        public int? P_18 { get; set; }

        public int? P_19 { get; set; }

        [StringLength(25)]
        public string IP_COMP { get; set; }

        [StringLength(25)]
        public string USER_COMP { get; set; }

        public int? SER_NO { get; set; }

        public int? CONTRACT_NO { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DATE_REV { get; set; }

        public int? MANUAL_NO { get; set; }

        public int? MANUAL_REG_ID { get; set; }

        [StringLength(50)]
        public string MANUAL_REG_NAM { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MANUAL_REG_DATE { get; set; }

        public int? MANUAL_REG_SER { get; set; }

        public int? FREE_PERCENT { get; set; }

        public int? FREE_OVER { get; set; }

        [StringLength(50)]
        public string FREE_PERCENT_NAM { get; set; }

        [StringLength(50)]
        public string FREE_OVER_NAM { get; set; }

        public int? CLAM_FIN_LOC_CO { get; set; }

        [StringLength(25)]
        public string LOC_TYPE { get; set; }

        public int? LOC_DISC { get; set; }

        public int? CLAM_FIN_IMP_CO { get; set; }

        [StringLength(25)]
        public string IMP_TYPE { get; set; }

        public int? IMP_DISC { get; set; }

        public int? OVR { get; set; }

        public int? MONY_RECIT { get; set; }

        [StringLength(500)]
        public string NOTE_RECIT { get; set; }
    }
}

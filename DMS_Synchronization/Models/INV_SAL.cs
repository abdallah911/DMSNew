namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Inv_Sal
    {
        public long? INVT_SEQ { get; set; }

        public long? INV_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? INV_DATE { get; set; }

        public long? INVT_NO { get; set; }

        [StringLength(250)]
        public string INVT_NAM { get; set; }

        [StringLength(25)]
        public string DOSAGE { get; set; }

        public int? CONS { get; set; }

        public int? SIZE_UNIT { get; set; }

        public int? UNIT { get; set; }

        public double? PACK_PRICE { get; set; }

        public double? PRICE_UNIT { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DOSE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DURATION { get; set; }

        [StringLength(25)]
        public string REPEAT { get; set; }

        public int? T_DURATION { get; set; }

        public int? COUNT { get; set; }

        public double? AMOUNT { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DATE_CHANGE { get; set; }

        [StringLength(25)]
        public string PART_NO { get; set; }

        public int? MANAGER { get; set; }

        [StringLength(50)]
        public string CARD_ID { get; set; }

        public int? MED_GROUP { get; set; }

        [StringLength(50)]
        public string GRUOP_TYPE { get; set; }

        [StringLength(10)]
        public string CLASS_CODE { get; set; }

        public long? PRINT_01 { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DATE_DURATION { get; set; }

        [StringLength(25)]
        public string CASH_ITEM { get; set; }

        [StringLength(25)]
        public string COVERED_ITEM { get; set; }

        [StringLength(1)]
        public string CANCEL_ITEM { get; set; }

        public double? TOT_PEND { get; set; }

        public double? TOT_TOT_PEND { get; set; }

        public double? ID_PHARM { get; set; }

        [StringLength(120)]
        public string NAME_PHARM { get; set; }

        public int? KIND_EX { get; set; }

        [StringLength(25)]
        public string ID_EX { get; set; }

        [StringLength(150)]
        public string NAME_EX { get; set; }

        public int? REPET_KIND { get; set; }

        public int? REPET_MONTH { get; set; }

        [Column(TypeName = "date")]
        public DateTime? REPE_DATE { get; set; }

        public double? YES_TOT { get; set; }

        public double? APP_TOT { get; set; }

        public double? CASH_TOT { get; set; }

        public int? PH_ID { get; set; }

        [StringLength(150)]
        public string PH_NAME { get; set; }

        public double? DISCOUNT_CLAIM { get; set; }

        public double? PAY_CLAIM { get; set; }

        public int? MAN_CLM { get; set; }

        [StringLength(150)]
        public string DISC_NAME { get; set; }

        [StringLength(25)]
        public string LIC_TYPE { get; set; }

        public int? LOC_PER { get; set; }

        public int? IMP_PER { get; set; }

        public int? P_CENT { get; set; }

        public double? AMOUNT_LOC { get; set; }

        public double? AMOUNT_IMP { get; set; }

        public double? AMOUNT_DISC { get; set; }

        [StringLength(25)]
        public string LOC_TYPE { get; set; }

        public int? FIN_LOC { get; set; }

        public int? LOC_DISC { get; set; }

        [StringLength(25)]
        public string IMP_TYPE { get; set; }

        public int? FIN_IMP { get; set; }

        public int? IMP_DISC { get; set; }
        public DateTime? REG_DATE { get; set; }
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
    }
}


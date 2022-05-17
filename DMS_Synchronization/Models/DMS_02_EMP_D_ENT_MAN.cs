
namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;

    public partial class DMS_02_EMP_D_ENT_MAN : BaseEntityDB
    {

        //public long Id { get; set; }
        public Nullable<decimal> D_ID { get; set; }
        public string CARD_ID { get; set; }
        public string MANAGER { get; set; }
        public int EMP_ID_ID { get; set; }
        public string EMP_SUB { get; set; }
        public int C_COMP_ID { get; set; }
        public string C_COMP_NAME { get; set; }
        public Nullable<System.DateTime> DATE_ROSHTA { get; set; }
        public int BATSH_NO { get; set; }
        public int REVIEW_ID { get; set; }
        public string REVIEW_NAME { get; set; }
        public string REV_DATE { get; set; }
        public string REV_TIME { get; set; }
        public double? MAN_LOC { get; set; }
        public double? MAN_IMP { get; set; }
        public double? MAN_TOT { get; set; }
        public string RECIT { get; set; }
        public double? DISC_LOC { get; set; }
        public double? DISC_IMP { get; set; }
        public double? DISC_TYPE_LOC { get; set; }
        public int EX_TYPE { get; set; }
        public Decimal RE_SEQ { get; set; }
        public Nullable<int> DISC_TYPE_IMP { get; set; }
        public double? PERCENT_MONY { get; set; }
        public int FREE_PERCENT_MONY { get; set; }
        public double? OVER_INSURANCE { get; set; }
        public int FREE_OVER_INSURANCE { get; set; }
        public double? P_1 { get; set; }
        public double? P_2 { get; set; }
        public double? P_3 { get; set; }
        public double? P_4 { get; set; }
        public double? LOC_WO { get; set; }
        public double? IMP_WO { get; set; }
        public double? PERCENT_MONY_M { get; set; }
        public double? P1 { get; set; }
        public double? P2 { get; set; }
        public double? P3 { get; set; }
        public double? P4 { get; set; }
        public int ID_STOP { get; set; }
        public int REVIEW_ID_STOP { get; set; }
        public string REV_DATE_STOP { get; set; }
        public double? OVER_PL { get; set; }
        public double? PERC_PL { get; set; }
        public double? TOT_DISC_EXP { get; set; }
        public double? MONY_OVER { get; set; }
        public int KIND { get; set; }
        public string CARD_NAM { get; set; }
        public int HOS_NAM { get; set; }
        public string EXP { get; set; }
        //public string TAKHSOS { get; set; }
        //public string TAKHSOS1 { get; set; }
        //public string TAKHSOS2 { get; set; }
        public string CONTRACT_NO { get; set; }
        public int MASS_NO { get; set; }
        public int RE_PROSECE { get; set; }
        public Decimal D_ID_OLD { get; set; }
        public Decimal CELING_ALL { get; set; }
        public int EDIT_CL { get; set; }
        public int KIND_KIND { get; set; }
        //public string FAM { get; set; }
        //public string CARD_ADD { get; set; }
        public double? SPECIAL_DISCOUNT { get; set; }
        public int ORDER_OK { get; set; }
        public int CELING_OK { get; set; }
        //public Nullable<long> CLM_HOS_NO { get; set; }
        public DateTime? REG_DATE { get; set; }
        public double? MONY_PROV { get; set; }
        public string D_EXP { get; set; }
        public string D_EXP_2 { get; set; }
        public string D_EXP_3 { get; set; }

        public double? Gross { get; set; }
        public double? PersonPayment { get; set; }
        public double? OverInsurance { get; set; }
        public double? TotalDiscount { get; set; }
        public double? Net { get; set; }
    }
}
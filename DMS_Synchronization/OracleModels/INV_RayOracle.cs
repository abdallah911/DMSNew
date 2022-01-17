using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.OracleModels
{
    public class INV_RayOracle
    {
        public long? INVT_SEQ { get; set; }

        public long? INV_ID { get; set; }

       
        public DateTime? INV_DATE { get; set; }

        public long? INVT_NO { get; set; }

        
        public string INVT_NAM { get; set; }

        
        public string DOSAGE { get; set; }

        public int? CONS { get; set; }

        public double? SIZE_UNIT { get; set; }

        public long? UNIT { get; set; }

        public double? PACK_PRICE { get; set; }

        public double? PRICE_UNIT { get; set; }

       
        public int DOSE { get; set; }

        public int DURATION { get; set; }

       
        public string REPEAT { get; set; }

        public int? T_DURATION { get; set; }

        public int? COUNT { get; set; }

        public double? AMOUNT { get; set; }

        
        public DateTime? DATE_CHANGE { get; set; }

       
        public string PART_NO { get; set; }

        public int? MANAGER { get; set; }

        
        public string CARD_ID { get; set; }

        public long? MED_GROUP { get; set; }

       
        public string GRUOP_TYPE { get; set; }

        
        public string CLASS_CODE { get; set; }

        public long? PRINT_01 { get; set; }

       
        public DateTime? DATE_DURATION { get; set; }

        
        public string CASH_ITEM { get; set; }

       
        public string COVERED_ITEM { get; set; }

        
        public string CANCEL_ITEM { get; set; }

        public double? TOT_PEND { get; set; }

        public double? TOT_TOT_PEND { get; set; }

        public double? ID_PHARM { get; set; }

        
        public string NAME_PHARM { get; set; }

        public int? KIND_EX { get; set; }

       
        public string ID_EX { get; set; }

        
        public string NAME_EX { get; set; }

        public int? REPET_KIND { get; set; }

        public int? REPET_MONTH { get; set; }

        
        public DateTime? REPE_DATE { get; set; }

        public double? YES_TOT { get; set; }

        public double? APP_TOT { get; set; }

        public double? CASH_TOT { get; set; }

        public long? DMS_CODE { get; set; }

        
        public long? PH_ID { get; set; }

        
        public string PH_NAME { get; set; }

        public double? DISCOUNT_CLAIM { get; set; }

        public double? PAY_CLAIM { get; set; }

        public int? MAN_CLM { get; set; }

        
        public string DISC_NAME { get; set; }

        
        public string LIC_TYPE { get; set; }

        public int? LOC_PER { get; set; }

        public int? IMP_PER { get; set; }

        public int? P_CENT { get; set; }

        public double? AMOUNT_LOC { get; set; }

        public double? AMOUNT_IMP { get; set; }

        public double? AMOUNT_DISC { get; set; }

        
        public DateTime? REG_DATE { get; set; }
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
}
}
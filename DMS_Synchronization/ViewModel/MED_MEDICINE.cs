using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.ViewModel
{
    public class MED_MEDICINE
    {
        public string MED_CODE { get; set; }
        public string CARD_NO { get; set; }
        public DateTime? RDATE { get; set; }
        public int MED_TYP { get; set; }
        public int DOSE { get; set; }
        public int NO_OF_UINT { get; set; }
        public double TOTAL_AMT { get; set; }
        public int MED_DURATION { get; set; }
        public int? TOT_DUR { get; set; }
        public int DOS_DUR { get; set; }
        public double? EXCESS { get; set; }
        public int PACK_SIZE { get; set; }
        public double PACK_PRICE { get; set; }
        public string CON_MED { get; set; }
        public int UNIT_NO { get; set; }
        public double UNIT_PRICE { get; set; }
        public string MED_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string ACT_MONTH { get; set; }
        public int? LFT_MONTH { get; set; }
        public DateTime? MONTH_DATE_STOP { get; set; }
        public DateTime? REG_DATE { get; set; }
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
    }
}
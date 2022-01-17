using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class ChronicViewModel
    {
        public long Id { get; set; }
        [DisplayName("Medicien Code")]
        public string MED_CODE { get; set; }
        public string CARD_NO { get; set; }
        public Nullable<System.DateTime> RDATE { get; set; }
        public Nullable<int> MED_TYP { get; set; }
        [DisplayName("Dose")]
        public Nullable<int> DOSE { get; set; }
        [DisplayName("Total Units")]
        public Nullable<int> NO_OF_UINT { get; set; }
        [DisplayName("Amount")]
        public Nullable<double> TOTAL_AMT { get; set; }
        [DisplayName("Medicien Duratuion")]
        public Nullable<int> MED_DURATION { get; set; }
        [DisplayName("Total Duratuion")]
        public string TOT_DUR { get; set; }
        [DisplayName("Dose Duratuion")]
        public Nullable<int> DOS_DUR { get; set; }
        public Nullable<int> EXCESS { get; set; }
        [DisplayName("Pack Size")]
        public Nullable<double> PACK_SIZE { get; set; }

        [DisplayName("Pack Price")]
        public Nullable<int> PACK_PRICE { get; set; }
        [DisplayName("Pack Price")]
        public Nullable<double> Des_PACK_PRICE { get; set; }
        public string CON_MED { get; set; }
        [DisplayName("Unit No")]
        public Nullable<int> UNIT_NO { get; set; }
        [DisplayName("Unit Price")]
        public Nullable<double> UNIT_PRICE { get; set; }
     
        [DisplayName("Medicien Name")]
        public string MED_NAME { get; set; }

        [DisplayName("Dosage Form")]
        public string DOSAGE_FORM { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string ACT_MONTH { get; set; }
        public Nullable<int> LFT_MONTH { get; set; }
        public string MONTH_DATE_STOP { get; set; }
    }
}
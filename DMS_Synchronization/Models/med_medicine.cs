using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class Med_Medicine:BaseEntityDB
    {
        public int Id { get; set; }
        public string MED_CODE { get; set; }
        public string CARD_NO { get; set; }
        public DateTime? RDATE { get; set; }
        public Decimal? MED_TYP { get; set; }
        public Decimal? DOSE { get; set; }
        public Decimal? NO_OF_UINT { get; set; }
        public Decimal? TOTAL_AMT { get; set; }
        public Decimal? MED_DURATION { get; set; }
        public string TOT_DUR { get; set; }
        public Decimal? DOS_DUR { get; set; }
        public Decimal? EXCESS { get; set; }
        public Decimal? PACK_SIZE { get; set; }
        public Decimal? PACK_PRICE { get; set; }
        public string CON_MED { get; set; }
        public Decimal? UNIT_NO { get; set; }
        public Decimal? UNIT_PRICE { get; set; }
        public string MED_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public string NOTES { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string ACT_MONTH { get; set; }
        public Decimal? LFT_MONTH { get; set; }
        public DateTime? MONTH_DATE_STOP { get; set; }
    }
}
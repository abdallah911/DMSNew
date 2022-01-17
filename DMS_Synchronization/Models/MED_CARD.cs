using DMS_Synchronization.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.Models
{
    public class Med_Card:BaseEntityDB
    {
        public string CARD_NO { get; set; }
        public Decimal PROVIDER_CODE { get; set; }
        public Decimal C_COMP_ID { get; set; }
        public string NOTES { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public DateTime? MONTH_START_DATE { get; set; }
        public DateTime? MONTH_END_DATE { get; set; }
        public Int64 GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public Int16 LOOK_01 { get; set; }
        public Decimal SEQ { get; set; }
        public Int64 PROVIDER_CODE_OLD { get; set; }
        public string TASHKHES_01 { get; set; }
        public Byte NO_PAY { get; set; }
        public Byte NO_OVER { get; set; }
        public Int16 ST_DAY { get; set; }
    }
}
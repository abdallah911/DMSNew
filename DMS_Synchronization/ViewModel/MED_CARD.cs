using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.ViewModel
{
    public class MED_CARD
    {
        public string CARD_NO { get; set; }
        public int PROVIDER_CODE { get; set; }
        public int C_COMP_ID { get; set; }
        public string NOTES { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public DateTime? MONTH_START_DATE { get; set; }
        public DateTime? MONTH_END_DATE { get; set; }
        public int GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public int? LOOK_01 { get; set; }
        public int? SEQ { get; set; }
        public int? PROVIDER_CODE_OLD { get; set; }
        public string TASHKHES_01 { get; set; }
        public int NO_PAY { get; set; }
        public int NO_OVER { get; set; }
        public int ST_DAY { get; set; }
        public DateTime? REG_DATE { get; set; }
        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }
    }
}
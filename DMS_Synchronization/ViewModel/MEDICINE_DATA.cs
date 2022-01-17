using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.ViewModel
{
    public class MEDICINE_DATA
    {
        public int? BRANCH_CODE { get; set; }
        public int? COMP_ID { get; set; }
        public string M_CODE { get; set; }
        public string LIC_TYPE { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public double? PACK_SIZE { get; set; }
        public double? PACK_PRICE { get; set; }
        public string M_TYPE { get; set; }
        public string CON_MED { get; set; }
        public int? UNIT_NO { get; set; }
        public double? UNIT_PRICE { get; set; }
        public string ACTIVE { get; set; }
        public string UPDATE_BY { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string GRN_CODE { get; set; }
        public DateTime? REG_DATE { get; set; }
        public int STOP_ID { get; set; }
        public int GRUOP_ID { get; set; }

        public int IS_SYNC { get; set; }
        public string SYNC_BY { get; set; }
        public DateTime? SYNC_DATE { get; set; }

    }
}
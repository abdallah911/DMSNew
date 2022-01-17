using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class Medicin
    {
        [DisplayName("Medicien Code")]
        public string M_CODE { get; set; }
        public string MED_GROUP { get; set; }
        public string TRADE_NAME { get; set; }
        public string DOSAGE_FORM { get; set; }
        public Nullable<int> PACK_SIZE { get; set; }
        public Nullable<decimal> PACK_PRICE { get; set; }
        public Nullable<int> UNIT_NO { get; set; }
        public Nullable<int> UNIT_PRICE { get; set; }
    }
}
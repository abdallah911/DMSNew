using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class Insurance
    {
        public int id { get; set; }
        public Nullable<decimal> INSURANCE_DAY { get; set; }
        public Nullable<decimal> INSURANCE_MONTH { get; set; }
        [DisplayName("Limit")]
        public string LIVEL { get; set; }
        public Nullable<decimal> CO_ID { get; set; }

    }
}
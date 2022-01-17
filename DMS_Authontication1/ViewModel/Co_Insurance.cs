using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class Co_Insurance
    {

        public int id { get; set; }
        public Nullable<decimal> CO_ID { get; set; }
       // [DisplayName("First Name")]
        public string LIVEL { get; set; }
        public Nullable<decimal> INSURANCE_DAY { get; set; }
        public Nullable<decimal> INSURANCE_MONTH { get; set; }
        public Nullable<decimal> LAB_DAY { get; set; }
        public Nullable<decimal> RAY_DAY { get; set; }
        [DisplayName("Daily limit")]
        public Nullable<decimal> INSURANCE_DAY_LAB { get; set; }
        [DisplayName("Monthly limit")]
        public Nullable<decimal> INSURANCE_MONTH_LAB { get; set; }
        public Nullable<decimal> COST_MONTHLY_YEAR { get; set; }
        public Nullable<decimal> COST_DALLY_YEAR { get; set; }
        public Nullable<decimal> NO_CLAEM_WEEK { get; set; }
        public Nullable<decimal> NO_CLEEM_YEAR { get; set; }
        public Nullable<decimal> FREE { get; set; }
        public Nullable<decimal> NO_CLM_DAY_YYYY { get; set; }
        public Nullable<decimal> MONY_CLM_DAY_YYYY { get; set; }
        public Nullable<decimal> NO_CLM_MON_YYYY { get; set; }
        public Nullable<decimal> MONY_CLM_MON_YYYY { get; set; }
        public Nullable<decimal> MED_DAY { get; set; }
        public Nullable<decimal> MED_MONTH { get; set; }
    }
}
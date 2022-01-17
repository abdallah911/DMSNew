using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class D_D_Emp
    {
        public int id { get; set; }
        [DisplayName("Company Percent")]
        public Nullable<int> CEILING_PERT { get; set; }
        public Nullable<int> COMP_ID { get; set; }
        public int EMP_CODE { get; set; }
        public string CARD_ID { get; set; }
        public Nullable<int> CONTRACT_NO { get; set; }
        public string CLASS_CODE { get; set; }

    }
}
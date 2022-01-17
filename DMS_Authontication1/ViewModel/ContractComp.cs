using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class ContractComp
    {
        [DisplayName("Company ID")]
        public int C_COMP_ID { get; set; }
        public Nullable<int> COMP_ID { get; set; }
        [DisplayName("Company Name")]
        public string C_ANAME { get; set; }
    }
}
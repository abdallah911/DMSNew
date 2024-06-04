using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST
{
    public class CompEmployees
    {
        [DisplayName("Card ID")]
        public string CARD_ID { get; set; }
        [DisplayName("Company ID")]
        public Nullable<int> C_COMP_ID { get; set; }
        [DisplayName("Name")]
        public string EMP_ANAME { get; set; }
        [DisplayName(" Name")]
        public string EMP_ENAME { get; set; }
        [DisplayName("First Name")]
        public string EMP_ANAME_ST { get; set; }
        [DisplayName("Secand Name")]
        public string EMP_ANAME_SC { get; set; }
        [DisplayName("Third Name")]
        public string EMP_ANAME_TH { get; set; }
        [DisplayName("Comp Name")]
        public string CompHolderName { get; set; }
        [DisplayName("Start Date")]
        public Nullable<System.DateTime> INS_START_DATE { get; set; }
        [DisplayName("End Date")]
        public Nullable<System.DateTime> INS_END_DATE { get; set; }
        [DisplayName("Birth Date")]
        public Nullable<System.DateTime> BIRTH_DATE { get; set; }

    }
}
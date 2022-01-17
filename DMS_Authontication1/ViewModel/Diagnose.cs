using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class Diagnose
    {
        public string DIAG_CODE { get; set; }
        public string special_id { get; set; }
        [DisplayName("Diagnosis")]
        public string DIAG_ANAME { get; set; }
    }
}
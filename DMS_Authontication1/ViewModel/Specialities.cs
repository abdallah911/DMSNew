using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class Specialities
    {
        [DisplayName("Special ID")]
        public string special_id { get; set; }
        [DisplayName("Speciality")]
        public string special_Aname { get; set; }
    }
}
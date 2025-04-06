using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class ChronicDetailsViewModel
    {
        public string MED_CODE { get; set; }
        public string MED_NAME { get; set; }
        public string DOSE { get; set; }
        public string MED_DURATION { get; set; }
        public string UNIT_NO { get; set; }        
        public string DOSAGE_FORM { get; set; }
        public string MONTH_DATE_STOP { get; set; }        
    }
}
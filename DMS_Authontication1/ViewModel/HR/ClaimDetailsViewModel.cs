using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class ClaimDetailsViewModel
    {
        public string Services { get; set; }
        public string SercvName { get; set; }
        public string ClaimSubmitted { get; set; }
        public string OverInsurance { get; set; }
        public string Discount { get; set; }
        public string ApprovAmount { get; set; }
        public string CopayAmt { get; set; }
        public string AfterCopay { get; set; }
        public string LocalAmount { get; set; }
        public string ImportAmount { get; set; }
        public string LocalDisc { get; set; }
        public string ImportDisc { get; set; }
        public string TotalDiscount { get; set; }
        public string Net { get; set; }

    }
}
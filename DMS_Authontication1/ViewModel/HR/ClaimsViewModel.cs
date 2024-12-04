using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class ClaimsViewModel
    {
        public string ClaimNo { get; set; }
        public string CreatedDate { get; set; }
        public string ClaimDate { get; set; }
        public string CardNo { get; set; }
        public string EmpName { get; set; }
        public string ProvName { get; set; }
        public string ProvType { get; set; }
        public string Diagnosis { get; set; }
        public string ServType { get; set; }
    }
}
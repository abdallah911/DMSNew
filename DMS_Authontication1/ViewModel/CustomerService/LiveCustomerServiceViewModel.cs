using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.CustomerService
{
    public class LiveCustomerServiceViewModel
    {
        public string ClaimNo { get; set; }
        public string ClaimDate { get; set; }
        public string ProviderId { get; set; }
        public string Branch { get; set; }
        public string Kind { get; set; }
        public string Total { get; set; }
        public string Copay { get; set; }
        public string Over { get; set; }
        public string Cash { get; set; }
        public string Credit { get; set; }
    }
}
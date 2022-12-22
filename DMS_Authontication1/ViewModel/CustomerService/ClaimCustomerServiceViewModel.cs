using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.CustomerService
{
    public class ClaimCustomerServiceViewModel
    {
        public string ClaimNo { get; set; }
        public string ClaimDate { get; set; }
        public string BatchNo { get; set; }
        public string GroupNo { get; set; }
        public string Gross { get; set; }
        public string Amount { get; set; }
        public string Notes { get; set; }
        public string F { get; set; }
    }
}
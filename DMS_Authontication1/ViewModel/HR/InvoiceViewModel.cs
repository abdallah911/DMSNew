using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class InvoiceViewModel
    {
        public string CompId { get; set; }
        public string CompName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CountOfBatch { get; set; }
        public string CountOfClaim { get; set; }
        public string Gross { get; set; }
        public string Net { get; set; }
        public string InvoiceNo { get; set; }
       // public string ServType { get; set; }
    }
}
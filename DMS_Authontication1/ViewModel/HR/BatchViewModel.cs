using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class BatchViewModel
    {
        public string BatchNumber { get; set; }
        public string ProviderID { get; set; }
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }
        public string CountOfClaim { get; set; }
        public string Gross { get; set; }
        public string Net { get; set; }      
    }
}
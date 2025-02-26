using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class ClaimsCardDetails
    {
        public string Claim_No { get; set; }
        public string Claim_Date { get; set; }
        public string Batch_No { get; set; }
        public string Provider_Name { get; set; }
        public string Serv_Name { get; set; }        
        public string Gross { get; set; }
        public string Net { get; set; }        
    }
}
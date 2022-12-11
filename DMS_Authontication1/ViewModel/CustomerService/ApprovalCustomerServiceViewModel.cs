using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.CustomerService
{
    public class ApprovalCustomerServiceViewModel
    {
       
        public string ApprovalNo { get; set; }
        public string ApprovalType { get; set; }
        public string Reply { get; set; }
        public string ApprovalAmount { get; set; }
        public string MedicalReply { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
   

      
    }
}
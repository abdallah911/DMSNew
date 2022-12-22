using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.CustomerService
{
    public class MonthlyCustomerServiceViewModel
    {
        public string CreatedDate { get; set; }
        public string ProviderCode { get; set; }
        public string UserN { get; set; }
        public string GroupName { get; set; }  
    }
}
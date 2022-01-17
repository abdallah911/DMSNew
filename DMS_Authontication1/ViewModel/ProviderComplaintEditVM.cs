using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class ProviderComplaintEditVM
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompName { get; set; }

        [Required]
        [Display(Name = "Complaint Description")]
        public string Problem { get; set; }


        public string RespobeStatus { get; set; }

        [Display(Name = " Date ")]
        public string ProblemDate { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        public string Status { get; set; }
        public string SolveProblem { get; set; }

        [Required]
        [Display(Name = "Provider Type ")]
        public int? ProviderTypeId { get; set; }
        public int? ProviderId { get; set; }
        public long? BranchId { get; set; }
        public string Subject { get; set; }
        public string ProviderTypeNew { get; set; }
        public string SERV_PROVIDERS_NEW { get; set; }
        public string Serv_Providers1 { get; set; }
    }
}
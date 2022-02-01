using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class RequestAddProvidersVM
    {
        public long ID { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompName { get; set; }

        [Required]
        [Display(Name = "Provider Name")]
        public string ServName { get; set; }

        [Required]
        [Display(Name = "Provider Address")]
        public string ServAddress { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Number Of People")]
        public int NumberOfPeople { get; set; }

        [Display(Name = "Provider Type")]
        public Nullable<int> ProviderType { get; set; }

        public string Country { get; set; }

        [Display(Name = " Reagion")]
        public string Region { get; set; }

        [Display(Name = "Responsible For")]
        public string ResponsableFor { get; set; }

        public string RespobeStatus { get; set; }

        public string Status { get; set; }
        public string ReasonRefuse { get; set; }

        [Display(Name = "Created Date")]
        public Nullable<System.DateTime> CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public Nullable<bool> IsSync { get; set; }

        public Nullable<System.DateTime> SyncDate { get; set; }

        public string SyncBy { get; set; }

        [Display(Name = "Provider Type Name")]
        public string ProviderTypeName { get; set; }
    }
}
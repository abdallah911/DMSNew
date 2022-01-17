using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.AfterSale
{
    public class AfterSaleViewModel
    {

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        [Required]
        [Display(Name = "Visit Reason")]
        public string VisitReason { get; set; }

        [Required]
        [Display(Name = "Person Name")]
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "Visit Date")]
        public Nullable<System.DateTime> VisitDate { get; set; }

        public string Region { get; set; }
        public string Phone { get; set; }
        public Nullable<bool> HasFeedBack { get; set; }
        public string FeedBackText { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> FeedBackDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<bool> IsNew { get; set; }
    }
}
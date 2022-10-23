using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.CustomerService
{
    public class CustomerServiceViewModel
    {

        [Required]
        [Display(Name = "Employee Name:")]
        public string EmployeeName { get; set; }

        [Required]
        [Display(Name = "Birth Date:")]
        public string BirthDate { get; set; }

        [Required]
        [Display(Name = "Age:")]
        public string Age { get; set; }

        [Required]
        [Display(Name = "Start Date:")]
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "End Date:")]
        public string EndDate { get; set; }
        [Required]
        [Display(Name = "Max Amount:")]
        public string MaxAmount { get; set; }
        [Required]
        [Display(Name = "Hospital Degree:")]
        public string HospitalDegree { get; set; }
        [Required]
        [Display(Name = "Exception Payment:")]
        public string ExceptionPayment { get; set; }
        [Required]
        [Display(Name = "Exception Over:")]
        public string ExceptionOver { get; set; }
        //public Nullable<bool> HasFeedBack { get; set; }
        //public string FeedBackText { get; set; }
        //public string Note { get; set; }
        //public Nullable<System.DateTime> FeedBackDate { get; set; }
        //public Nullable<System.DateTime> CreatedDate { get; set; }
        //public Nullable<System.DateTime> UpdatedDate { get; set; }
        //public Nullable<System.DateTime> DeletedDate { get; set; }
        //public string CreatedBy { get; set; }
        //public string UpdatedBy { get; set; }
        //public string DeletedBy { get; set; }
        //public Nullable<bool> IsDeleted { get; set; }
        //public Nullable<bool> IsNew { get; set; }
    }
}
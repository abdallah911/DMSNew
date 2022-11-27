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
        [Display(Name = "Card ID:")]
        public string CardID { get; set; }

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
        [Display(Name = "Class Name:")]
        public string ClassName { get; set; }
        [Required]
        [Display(Name = "Hospital Degree:")]
        public string HospitalDegree { get; set; }
        [Required]
        [Display(Name = "Exception Payment:")]
        public string ExceptionPayment { get; set; }
        [Required]
        [Display(Name = "Exception Over:")]
        public string ExceptionOver { get; set; }
        
        [Display(Name = "Old Card:")]
        public string OldCard { get; set; }
        [Display(Name = "National Id:")]
        public string NationalId { get; set; }
        [Display(Name = "Medical Network:")]
        public string MedicalNetwork { get; set; }
        [Display(Name = "Card Color:")]
        public string CardColor { get; set; }
        [Display(Name = "Mobile 1:")]
        public string Mobile1 { get; set; }
        [Display(Name = "Mobile 2:")]
        public string Mobile2 { get; set; }

        [Display(Name = "Medication Claims:")]
        public string MedicationClaims { get; set; }
        [Display(Name = "Medication Claims Under Review:")]
        public string MedicationClaimsUnderReview { get; set; }
        [Display(Name = "Other Consumption:")]
        public string OtherConsumption { get; set; }
        [Display(Name = "All Consumption:")]
        public string AllConsumption { get; set; }

        [Display(Name = "Remaining:")]
        public string Remaining { get; set; }

        [Display(Name = "Percent:")]
        public string Percent { get; set; }

        [Display(Name = "Approval Consumption:")]
        public string ApprovalConsumption { get; set; }

        [Display(Name = "Count Claims:")]
        public string CountClaims { get; set; }
        [Display(Name = "Total Gross Claims:")]
        public string TotalGrossClaims { get; set; }
        [Display(Name = "Total Net Claims:")]
        public string TotalNetClaims { get; set; }

        [Display(Name = "Count Approvals:")]
        public string CountApprovals { get; set; }
        
        [Display(Name = "Total Amount Approvals:")]
        public string TotalAmountApprovals { get; set; }

        [Display(Name = "Count Indemnity:")]
        public string CountIndemnity { get; set; }
        [Display(Name = "Total Gross Indemnity:")]
        public string TotalGrossIndemnity { get; set; }
        [Display(Name = "Total Net Indemnity:")]
        public string TotalNetIndemnity { get; set; }
        [Display(Name = "Count Monthly:")]
        public string CountMonthly { get; set; }
        [Display(Name = "Count:")]
        public string CountLive { get; set; }
        [Display(Name = "Total:")]
        public string TotalLive { get; set; }
        [Display(Name = "Total Credit:")]
        public string TotalCredit { get; set; }
       // public string Flag { get; set; }
        public string NotesCloseCard { get; set; }
        //public string flgPhone { get; set; }
        //[Display(Name = "Remaining:")]
        //public string Remaining { get; set; }
        //
        //[Display(Name = "Percent:")]
        //public string Percent { get; set; }

        //[Display(Name = "Approval Consumption:")]
        //public string ApprovalConsumption { get; set; }


        //[Display(Name = "Approval Consumption:")]
        //public string ApprovalConsumption { get; set; }


        //Consumption
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
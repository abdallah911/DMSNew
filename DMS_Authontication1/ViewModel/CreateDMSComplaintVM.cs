using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class CreateDMSComplaintVM
    {
        [Required]
        [Display(Name = "Company Name")]
        public string CompName { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [Display(Name = " Complaint Reason ")]
        public string ComplaintReason { get; set; }


        [Required]
        [Display(Name = "Complaint Description")]
        public string ComplaintDescription { get; set; }

        [Display(Name = " Employee Name ")]
        public string EmployeeName { get; set; }

        public string RespobeStatus { get; set; }

        [Display(Name = " Date ")]
        public string CreatedDate { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Status { get; set; }
        public string SolveProblem { get; set; }


    }
}
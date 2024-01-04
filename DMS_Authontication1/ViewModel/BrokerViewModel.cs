using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class BrokerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "The national Id must be exactly 14 digits.")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        [Range(0, long.MaxValue, ErrorMessage = "MembershipNo cannot be less than zero.")]
        public long MembershipNo { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "TaxCardId cannot be less than zero.")]

        [Required(ErrorMessage = "The field is required.")]
        public long TaxCardId { get; set; }
        [Required(ErrorMessage = "The field is required.")]
        public HttpPostedFileBase TaxCard { get; set; }
        public string TaxCardName { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        public HttpPostedFileBase PracticeCard { get; set; }
        public string PracticeCardName { get; set; }
        public DateTime? CreatedOn { get; set; }

        // Additional properties for relationships or display purposes
        [Required(ErrorMessage = "The field is required.")]

        public bool? IsCompany { get; set; }
        [ConditionalRequired(nameof(IsCompany), true, ErrorMessage = "Company Name is required.")]
        public string CompanyName { get; set; }

        // Constructor to set default values or initialize collections
        public BrokerViewModel()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
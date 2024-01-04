using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class Broker
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The field is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The field is required.")]
        //using regex to make sure the national Id is 14 digits only 
        [RegularExpression(@"^\d{14}$", ErrorMessage = "The national Id must be exactly 14 digits.")]
        public string NationalId { get; set; }
        [Required(ErrorMessage = "The field is required.")]
        public long MembershipNo { get; set; }
        [Required(ErrorMessage = "The field is required.")]
        public long TaxCardId { get; set; }
        public string TaxCardName { get; set; }
        public string PracticeCardName { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; } = DateTime.Now;

        //----------relations---------//
        [Required(ErrorMessage = "The field is required.")]
        public bool IsCompany { get; set; }
        public string CompanyName { get; set; }
        //public Company Company { get; set; }
        //[ForeignKey("Company")]
        //public int? ComapnyId { get; set; }

    }
}
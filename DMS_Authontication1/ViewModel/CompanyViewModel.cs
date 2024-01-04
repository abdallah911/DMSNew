using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class CompanyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The field is required.")]
        public string Name { get; set; }

        public DateTime? CreatedOn { get; set; }

        // Additional properties for relationships or display purposes
        public int BrokerId { get; set; }

        // Constructor to set default values or initialize collections
        public CompanyViewModel()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
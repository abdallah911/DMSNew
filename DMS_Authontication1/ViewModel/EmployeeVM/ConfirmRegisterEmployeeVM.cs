using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.EmployeeVM
{
    public class ConfirmRegisterEmployeeVM
    {
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Required]
        [Display(Name = " Full Name ")]
        [RegularExpression(@"^([a-zA-Z-ء-ي \.\&\'\-]+)$", ErrorMessage = "Invalid Last Name")]

        public string FullName { get; set; }

        //[Required]
        [EmailAddress]
        [Display(Name = " Email ")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Card ID ")]
        public string CardId { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "National Id")]
        public string NationalId { get; set; }
        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

    }
}
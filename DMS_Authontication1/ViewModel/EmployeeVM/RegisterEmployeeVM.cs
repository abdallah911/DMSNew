using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.EmployeeVM
{
    public class RegisterEmployeeVM
    {
        [Required]
        [Display(Name = "User Name")]

        public string UserName { get; set; }

        [Required]
        [Display(Name = " Full Name ")]
        [RegularExpression(@"^([a-zA-Z-ء-ي \.\&\'\-]+)$", ErrorMessage = "Invalid Last Name")]

        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = " Email ")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "National ID ")]
        public string NationalId { get; set; }

        [Required]
        [Display(Name = "Card ID ")]
        public string CardId { get; set; }

        [Required]
        [Display(Name = "Birth Date ")]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 11)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [Phone]
        [StringLength(11, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 11)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Confirm Phone")]
        [Compare("PhoneNumber", ErrorMessage = "The Phone and confirmation Phone do not match.")]
        public string ConfirmPhone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string ConfirmType { get; set; }

        public string NationalIdImage { get; set; }

        [Required]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}
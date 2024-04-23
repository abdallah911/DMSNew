using DMS_Authontication1.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class ProposalMainViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        [Required]
        public string CompanyEnglishName { get; set; }
        [Required]

        public string CompanyArabicName { get; set; }
        //[Range(100, long.MaxValue, ErrorMessage = "The number can't.")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "The phone number must be 8 digits.")]
        public string CompanyTelePhone { get; set; }
        [Required]

        public string AdministratorName { get; set; }
        [Required]
        //[Range(100, long.MaxValue, ErrorMessage = "The number must be at least 3 digits.")]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "The phone number must be eleven digits and can start with a 01.")]
        public string AdministratorPhone { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [CustomValidation(typeof(ProposalMainViewModel), "ValidateContractDate")]
        public DateTime ContractDate { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Doctor visits count cannot be less than zero.")]
        public int DoctorVisitsCount { get; set; }
        [Required]
        [Range(0, 24, ErrorMessage = "Doctor visits duration must be in range 0 - 24.")]
        public decimal DoctorVisitsDuration { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Required]
        public string Email { get; set; }
        [Required]

        public bool InsuranceExists { get; set; }
        //[Required]

        [Range(0, int.MaxValue, ErrorMessage = "Medication Pool Value cannot be less than zero.")]

        public int? MedicationPoolVal { get; set; } = 0;
        //[Required]

        [Range(0, 100, ErrorMessage = "Medication pool percentage must be between 0 and 100.")]
        public decimal? MedicationPoolPercentage { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Prex Pool Value cannot be less than zero.")]

        public int? PrexPoolVal { get; set; } = 0;
        [Range(0, 100, ErrorMessage = "Prex pool percentage must be between 0 and 100.")]

        public decimal? PrexPoolPercentage { get; set; } = 0;

        public ProposalCoverage PrexPoolCoverage { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Exception Pool Value cannot be less than zero.")]

        public int? ExceptionPoolVal { get; set; } = 0;
        [Range(0, 100, ErrorMessage = "Exception pool percentage must be between 0 and 100.")]

        public decimal? ExceptionPoolPercentage { get; set; } = 0;

        public ProposalCoverage ExceptionPoolCoverage { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Chronic Pool Value cannot be less than zero.")]

        public int? ChronicPoolVal { get; set; } = 0;
        [Range(0, 100, ErrorMessage = "Chronic pool percentage must be between 0 and 100.")]

        public decimal? ChronicPoolPercentage { get; set; } = 0;

        public ProposalCoverage ChronicPoolCoverage { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Critical Pool Value cannot be less than zero.")]

        public int? CriticalPoolVal { get; set; } = 0;
        [Range(0, 100, ErrorMessage = "Critical pool percentage must be between 0 and 100.")]

        public decimal? CriticalPoolPercentage { get; set; } = 0;

        public ProposalCoverage CriticalPoolCoverage { get; set; }


        // For file upload
        public HttpPostedFileBase ConsumptionFile { get; set; }
        public string ConsumptionFileName { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Categories must be at least 1")]
        public int CategoriesCount { get; set; }

        // Relations
        [Required]

        public bool IsBroker { get; set; }
        [ConditionalRequired(nameof(IsBroker), true, ErrorMessage = "please choose a broker.")]

        public int? BrokerId { get; set; }
        [ConditionalRequired(nameof(IsBroker), true, ErrorMessage = "Broker Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Broker Percentage must be between 0 and 100.")]
        public decimal? BrokerPercentage { get; set; }
        public string BrokerName { get; set; }

        public string UserId { get; set; }
        [Required(ErrorMessage = "please choose a company activity.")]

        public int CompanyActivityId { get; set; }
        public string CompanyActivityName { get; set; }

        [Required(ErrorMessage = "please choose an Area.")]


        public int AreaId { get; set; }
        public string AreaName { get; set; }

        // Additional properties for step two, step three, etc.
        // You can extend this view model to include properties related to step two and step three if needed.


        //custom validation 
        public static ValidationResult ValidateContractDate(DateTime contractDate, ValidationContext validationContext)
        {
            if (contractDate < DateTime.Now.Date)
            {
                return new ValidationResult("Contract date cannot be in the past.");
            }

            return ValidationResult.Success;
        }
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
        public class EightDigitAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    long number = (long)value;

                    // Check if the number has exactly 8 digits
                    if (number >= 10000000 && number <= 99999999)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("The phone number must be exactly 8 digits.");
                    }
                }

                return ValidationResult.Success; // or ValidationResult.Success if null is allowed
            }
        }
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
        public class ElevenDigitPhoneAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    string phoneNumber = value.ToString();

                    // Check if the phone number has exactly 11 digits and starts with "01"
                    if (phoneNumber.Length == 11 && phoneNumber.StartsWith("01") && phoneNumber.All(char.IsDigit))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("The phone number must be eleven digits and start with '01'.");
                    }
                }

                return ValidationResult.Success; // or ValidationResult.Success if null is allowed
            }
        }
    }
}
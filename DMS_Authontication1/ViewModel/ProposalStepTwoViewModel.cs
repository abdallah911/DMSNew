using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DMS_Authontication1.Models;
namespace DMS_Authontication1.ViewModel
{
    public class ProposalStepTwoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Annual Coverage Ceiling is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Annual Coverage Ceiling must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Annual Coverage Ceiling format.")]
        public decimal AnnualCoverageCeiling { get; set; }

        [Required(ErrorMessage = "Participants Count is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Participants Count must be greater than or equal to zero.")]
        public int ParticipantsCount { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Min Age is required.")]
        [Range(0, 200, ErrorMessage = "Min Age must be between 0 and 200.")]
        public int MinAge { get; set; }

        [Required(ErrorMessage = "Max Age is required.")]
        [Range(0, 200, ErrorMessage = "Max Age must be between 0 and 200.")]
        public int MaxAge { get; set; }

        //[Required(ErrorMessage = "Avg Age is required.")]
        //[Range(0, 200, ErrorMessage = "Avg Age must be between 0 and 200.")]
        //public int AvgAge { get; set; }
        [Required(ErrorMessage = "Please provide the approval status for Checks Inside Hospital.")]
        public ApprovalStepTwo ChecksInsideHospital { get; set; }

        [Required(ErrorMessage = "Please provide the approval status for Physical Therapy Inside Hospital.")]
        public ApprovalStepTwo PhysicalTherapyInsideHospital { get; set; }

        [Required(ErrorMessage = "Please provide the approval status for Outside Clinic Inside Hospital.")]
        public ApprovalStepTwo OutsideClinicInsideHospital { get; set; }

        [Required(ErrorMessage = "Please provide the approval status for Dental Services Inside Hospital.")]
        public ApprovalStepTwo DentalServicesInsideHospital { get; set; }

        public string EmployeesDataFileName { get; set; }
        //[Required(ErrorMessage = "Please select a file.")]

        public HttpPostedFileBase EmployeesDataFile { get; set; }
        public int ClassCode { get; set; }
        [Required(ErrorMessage = "Accidents is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Accidents must be greater than or equal to 0.")]
        public int Accidents { get; set; }
        [Required(ErrorMessage = "Death is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Death must be greater than or equal to 0.")]

        public int Death { get; set; }
        //----------relations--------------//
        [Required(ErrorMessage = "Please provide the Average Age.")]
        public int AgeAvgId { get; set; }
        [Required(ErrorMessage = "Please provide the Card Color.")]
        public int CardColorId { get; set; }

        [Required(ErrorMessage = "Please provide the Residence Degree.")]
        public int ResidenceDegreeId { get; set; }

        [Required(ErrorMessage = "Please provide the Medical Network.")]
        public int MedicalNetworkId { get; set; }
        [Required]

        public int ProposalMainId { get; set; }
    }
}
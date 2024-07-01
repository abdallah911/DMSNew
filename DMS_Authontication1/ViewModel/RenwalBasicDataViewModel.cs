using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DMS_Authontication1.Models;
namespace DMS_Authontication1.ViewModel
{
    public class RenwalBasicDataViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Value Pool is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Value Pool Ceiling must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Value Pool format.")]
        public double ValuePool { get; set; }
               
        [Required(ErrorMessage = "Percent Pool is required.")]
        [Range(0, 100, ErrorMessage = "Percent Pool must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Percent Pool format.")]
        public int PercentPool { get; set; }
        public int TypeCovarge { get; set; }
        
        [Required(ErrorMessage = "Stop Loss is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Stop Loss must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Stop Loss format.")]
        public double StopLoss { get; set; }
        
        [Required(ErrorMessage = "Visitor Value is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Visitor Value must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Visitor Value format.")]
        public double VisitorValue { get; set; }
        
        [Required(ErrorMessage = "Visitor Number is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Visitor Number must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Visitor Number format.")]
        public int VisitorNumber { get; set; }
        public int VisitorType { get; set; }
        public bool IsBroker { get; set; }
        public int? BrokerId { get; set; }
        
        //[Required(ErrorMessage = "Broker Percentage is required.")]
        //[Range(0, 100, ErrorMessage = "Broker Percentage must be greater than or equal to zero.")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Broker Percentage format.")]
        public int? BrokerPercentage { get; set; }

        [Required(ErrorMessage = "Issuance Expenses is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Issuance Expenses must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Issuance Expenses format.")]
        public double IssuanceExpenses { get; set; }

        [Required(ErrorMessage = "Admin Expenses is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Admin Expenses must be greater than or equal to zero.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid Admin Expenses format.")]
        public double AdminExpenses { get; set; }
        public bool IsMedication { get; set; }
        public bool IsInpatient { get; set; }
        public bool IsLab { get; set; }
        public int CompId { get; set; }
        public int ContractNo { get; set; }
        public int CountClass { get; set; }
        public int typAction { get; set; }
        public int MainId { get; set; }        
        public int MainIdOld { get; set; }

        public double ValueAllOld { get; set; }
        public double ValueMedicationOld { get; set; }
        public double StopLossOld { get; set; }
        public double LossRatioOld { get; set; }
        public double ExpectedLossRatioOld { get; set; }
        public List<int> poolService { get; set; }
        public string Notes { get; set; }
    }

}
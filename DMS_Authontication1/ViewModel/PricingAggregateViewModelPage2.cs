using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DMS_Authontication1.ViewModel
{
    public class PricingAggregateViewModelPage2
    {
        public int ProposalMainId {get; set;}
        public List<ProposalStepTwo> ProposalStepTwoes { get; set; }
        // Add additional properties for new prices
        [Required]
        [DecimalRange(0, double.MaxValue, ErrorMessage = "Each price should be in the range 0 to Double.MaxValue.")]
        public List<double> NewPrices { get; set; }
        public int Catcount { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DecimalRangeAttribute : ValidationAttribute
    {
        private readonly double _min;
        private readonly double _max;

        public DecimalRangeAttribute(double min, double max)
        {
            //// Perform a range check to avoid overflow exception
            //if (min < (double)decimal.MinValue || max > (double)decimal.MaxValue)
            //{
            //    throw new ArgumentOutOfRangeException(nameof(min) + " or " + nameof(max), "Values are outside the valid range for decimal.");
            //}
            _min = min;
            _max = max;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is List<double> prices)
            {
                foreach (var price in prices)
                {
                    if (price < _min || price > _max)
                    {
                        return new ValidationResult($"Each price should be in the range {_min} to {_max}.");
                    }
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Invalid data type for validation attribute.");
        }
    }
}
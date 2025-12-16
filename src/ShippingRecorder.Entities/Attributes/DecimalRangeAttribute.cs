using System;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DecimalRangeAttribute : ValidationAttribute
    {
        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; } = decimal.MaxValue;

        public DecimalRangeAttribute(short minimum)
            => Minimum = minimum;

        public DecimalRangeAttribute(int minimum)
            => Minimum = minimum;

        public DecimalRangeAttribute(long minimum)
            => Minimum = minimum;

        public DecimalRangeAttribute(float minimum)
            => Minimum = (decimal)minimum;

        public DecimalRangeAttribute(double minimum)
            => Minimum = (decimal)minimum;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Required values should be checked using [Required], not here
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (decimal.TryParse(value.ToString(), out decimal result) &&
                (result >= Minimum) &&
                (result <= Maximum))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"{validationContext.DisplayName} must be a decimal between {Minimum} and {Maximum}");
        }
    }
}
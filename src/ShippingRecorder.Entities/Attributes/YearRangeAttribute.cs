using System;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class YearRangeAttribute : ValidationAttribute
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; } = DateTime.Today.Year;

        public YearRangeAttribute(int minimum)
            => Minimum = minimum;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Required values should be checked using [Required], not here
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (int.TryParse(value.ToString(), out int result) &&
                (result >= Minimum) &&
                (result <= Maximum))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"{validationContext.DisplayName} must be a year between {Minimum} and {Maximum}");
        }
    }
}
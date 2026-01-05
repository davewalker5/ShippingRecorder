using System;
using System.ComponentModel.DataAnnotations;

namespace ShippingRecorder.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class LongRangeAttribute : ValidationAttribute
    {
        public long Minimum { get; set; }
        public long Maximum { get; set; } = long.MaxValue;

        public LongRangeAttribute(long minimum, string error)
        {
            Minimum = minimum;
            ErrorMessage = error;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Required values should be checked using [Required], not here
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (long.TryParse(value.ToString(), out long result) &&
                (result >= Minimum) &&
                (result <= Maximum))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
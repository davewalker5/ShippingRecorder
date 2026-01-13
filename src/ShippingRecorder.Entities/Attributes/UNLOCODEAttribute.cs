using ShippingRecorder.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ShippingRecorder.Entities.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UNLOCODEAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the properties of the object instance being validated
            var properties = validationContext.ObjectInstance.GetType().GetProperties();

            // See if the object has the named property (at this point, it should have)
            var property = properties.FirstOrDefault(x => x.Name == validationContext.MemberName);
            if (property != null)
            {
                // It does, so get the value (the UN/LOCODE) and retrieve the port with that code
                var code = ((string)property.GetValue(validationContext.ObjectInstance) ?? "").ToUpper();
                var retriever = validationContext.GetService(typeof(IPortsRetriever)) as IPortsRetriever;
                var port = retriever.GetAsync(code).Result;

                if (port == null)
                {
                    return new ValidationResult($"{code} is not a valid UN/LOCODE code for {validationContext.MemberName}");
                }
            }

            return ValidationResult.Success;
        }
    }
}

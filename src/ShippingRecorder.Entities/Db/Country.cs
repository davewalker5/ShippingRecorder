using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Country : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Code")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code must be 2 digits long")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Country code must contain letters only")]
        [Required(ErrorMessage = "You must provide a country code")]
        public string Code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        public override string ToString()
            => $"{Code} - {Name}";
    }
}
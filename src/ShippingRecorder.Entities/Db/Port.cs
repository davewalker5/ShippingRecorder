using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Port : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "You must specify a country")]
        public long CountryId { get; set; }

        [DisplayName("UN/LOCODE")]
        [Required(ErrorMessage = "You must specify a UN/LOCODE")]
        public string Code { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must specify a port name")]
        public string Name { get; set; }

        public virtual Country Country { get; set; }

        public override string ToString()
            => $"{Code} - {Name}, {Country?.Name ?? CountryId.ToString()}";
    }
}
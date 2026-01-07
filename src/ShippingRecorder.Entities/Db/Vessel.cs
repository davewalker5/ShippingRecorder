using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ShippingRecorder.Entities.Attributes;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Vessel : ShippingRecorderEntityBase
    {
        public const int EarliestYearBuilt = 1900;
        public const decimal MinimumDraught = 2;
        public const int MinimumLength = 5;
        public const int MinimumBeam = 2;

        [Key]
        public long Id { get; set; }

        [DisplayName("IMO")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "IMO must be 7 digits long")]
        [RegularExpression(@"^\d+$", ErrorMessage = "IMO must contain digits only")]
        [Required(ErrorMessage = "You must provide an IMO")]
        public string IMO { get; set; }

        [DisplayName("Built")]
        [YearRange(1900)]
        public int? Built { get; set; }

        [DisplayName("Draught")]
        [DecimalRange(1)]
        public decimal? Draught { get; set; }

        [DisplayName("Length")]
        [DecimalRange(1)]
        public int? Length { get; set; }

        [DisplayName("Beam")]
        [DecimalRange(1)]
        public int? Beam { get; set; }

        public ICollection<RegistrationHistory> RegistrationHistory { get; set; } = [];

        [NotMapped]
        public RegistrationHistory ActiveRegistrationHistory => RegistrationHistory?.FirstOrDefault(x => x.IsActive);
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class Vessel
    {
        public const int EarliestYearBuilt = 1900;
        public const decimal MinimumDraught = 2;
        public const int MinimumLength = 5;
        public const int MinimumBeam = 2;

        [Key]
        public long Id { get; set; }

        [DisplayName("IMO")]
        [Required(ErrorMessage = "You must provide an IMO")]
        public string IMO { get; set; }

        [DisplayName("Built")]
        public int? Built { get; set; }

        [DisplayName("Draught")]
        public decimal? Draught { get; set; }

        [DisplayName("Length")]
        public int? Length { get; set; }

        [DisplayName("Beam")]
        public int? Beam { get; set; }

        public ICollection<RegistrationHistory> RegistrationHistory { get; set; } = [];
    }
}
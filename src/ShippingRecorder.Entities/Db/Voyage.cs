using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Voyage : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Operator")]
        [Required(ErrorMessage = "You must specify an operator")]
        public long OperatorId { get; set; }

        [DisplayName("Vessel")]
        [Required(ErrorMessage = "You must specify a vessel")]
        public long VesselId { get; set; }

        [DisplayName("Number")]
        [Required(ErrorMessage = "You must provide a voyage number")]
        public string Number { get; set; }

        public virtual Operator Operator { get; set; }
        public virtual Vessel Vessel { get; set; }
        public ICollection<VoyageEvent> Events { get; set; } = [];

        public override string ToString()
            => $"{Operator?.Name ?? OperatorId.ToString()} : {Number}";
    }
}
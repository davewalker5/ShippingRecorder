using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

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

        /// <summary>
        /// Return an ordered list of voyage events. Currently, this assumes only one arrival
        /// and departure per port per day, as there's no sequence number on events
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VoyageEvent> OrderedEvents()
            => Events?.OrderBy(x => x.Date).ThenByDescending(x => x.EventType);

        public VoyageEvent FirstEvent()
            => OrderedEvents()?.FirstOrDefault();

        public VoyageEvent LastEvent()
            => OrderedEvents()?.LastOrDefault();

        public override string ToString()
        {
            // Start with the operator name
            StringBuilder builder = new();
            builder.Append(Operator?.Name ?? OperatorId.ToString());

            // If we have any events, add the first event date
            var evt = FirstEvent();
            if (evt != null)
            {
                builder.Append(" : ");
                builder.Append(FirstEvent()?.Date.ToShortDateString());
            }

            // Finish with the voyage number
            builder.Append(" : ");
            builder.Append(Number);

            return builder.ToString();
        }
    }
}
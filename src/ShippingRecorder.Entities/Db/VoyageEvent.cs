using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class VoyageEvent : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Voyage")]
        [Required(ErrorMessage = "You must specify a voyage")]
        public long VoyageId { get; set; }

        [DisplayName("Event Type")]
        [Required(ErrorMessage = "You must provide an event type")]
        public VoyageEventType EventType { get; set; }

        [DisplayName("Port")]
        [Required(ErrorMessage = "You must provide a port")]
        public long PortId { get; set; }

        [DisplayName("Date")]
        [Required(ErrorMessage = "You must provide an event date")]
        public DateTime Date { get; set; }

        public virtual Port Port { get; set; }

        public override string ToString()
            => $"{Date} : {Port} : {EventType}";
    }
}
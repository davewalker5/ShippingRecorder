using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public partial class Sighting : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }
        public long LocationId { get; set; }
        public long? VoyageId { get; set; }
        public long VesselId { get; set; }
        public DateTime Date { get; set; }
        public bool IsMyVoyage { get; set; }

        public virtual Vessel Vessel { get; set; }
        public virtual Voyage Voyage { get; set; }
        public virtual Location Location { get; set; }

        public string FormattedDate { get { return Date.ToString("dd-MMM-yyyy"); } }
    }
}
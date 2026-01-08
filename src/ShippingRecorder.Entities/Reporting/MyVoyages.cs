using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class MyVoyages
    {
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string IMO { get; set; }
        public string Vessel { get; set; }
        public long VesselId { get; set; }

        public string FormattedDate { get { return Date.ToString("dd-MMM-yyyy"); } }
    }
}

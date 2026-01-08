using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class SightingsByMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Sightings { get; set; }
        public int? Locations { get; set; }
        public int? Vessels { get; set; }
    }
}

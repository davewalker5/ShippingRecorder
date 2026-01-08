using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class FlagStatistics
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Sightings { get; set; }
        public int? Locations { get; set; }
        public int? Vessels { get; set; }
    }
}

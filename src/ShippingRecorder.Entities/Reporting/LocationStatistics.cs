using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Reporting
{
    [Keyless]
    [ExcludeFromCodeCoverage]
    public class LocationStatistics
    {
        public string Name { get; set; }
        public int? Sightings { get; set; }
        public int? Vessels { get; set; }
    }
}

using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Config.Entities
{
    [ExcludeFromCodeCoverage]
    public class ApiRoute
    {
        public string Name { get; set; }
        public string Route { get; set; }
    }
}

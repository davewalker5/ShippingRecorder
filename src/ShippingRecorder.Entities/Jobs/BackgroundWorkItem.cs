using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Jobs
{
    [ExcludeFromCodeCoverage]
    public class BackgroundWorkItem
    {
        public string JobName { get; set; }

        public override string ToString()
            => $"JobName = {JobName}";
    }
}

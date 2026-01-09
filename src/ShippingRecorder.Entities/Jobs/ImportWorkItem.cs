using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Jobs
{
    [ExcludeFromCodeCoverage]
    public class ImportWorkItem : BackgroundWorkItem
    {
        public string FileName { get; set; }
        public string Content { get; set; }

        public override string ToString()
            => $"{base.ToString()}, FileName = {FileName}, Content Length = {Content.Length}";
    }
}

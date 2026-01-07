using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class JobStatus : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Error { get; set; }
    }
}

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Config
{
    [ExcludeFromCodeCoverage]
    public class CommandLineOptionValue
    {
        public CommandLineOption Option { get; set; }
        public List<string> Values { get; private set; } = [];
    }
}

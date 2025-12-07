using System.Collections.Generic;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IHelpGenerator
    {
        void Generate(IEnumerable<CommandLineOption> options);
    }
}

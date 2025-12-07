using System.Collections.Generic;
using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface ICommandLineParser
    {
        void Add(CommandLineOptionType optionType, bool mandatory, string name, string shortName, string description, int minimumNumberOfValues, int maximumNumberOfValues);
        List<string> GetValues(CommandLineOptionType optionType);
        void Help();
        bool IsPresent(CommandLineOptionType optionType);
        void Parse(IEnumerable<string> args);
    }
}
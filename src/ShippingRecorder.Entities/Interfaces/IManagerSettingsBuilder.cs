using ShippingRecorder.Entities.Config;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IManagerSettingsBuilder
    {
        ShippingRecorderApplicationSettings BuildSettings(ICommandLineParser parser, string configJsonPath);
    }
}
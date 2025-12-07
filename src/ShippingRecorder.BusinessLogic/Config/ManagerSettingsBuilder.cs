using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.BusinessLogic.Config
{
    public class ManagerSettingsBuilder : ConfigReader<ShippingRecorderApplicationSettings>, IManagerSettingsBuilder
    {
        /// <summary>
        /// Construct the application settings from the configuration file and any command line arguments
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="configJsonPath"></param>
        /// <returns></returns>
        public ShippingRecorderApplicationSettings BuildSettings(ICommandLineParser parser, string configJsonPath)
            => base.Read(configJsonPath);
    }
}
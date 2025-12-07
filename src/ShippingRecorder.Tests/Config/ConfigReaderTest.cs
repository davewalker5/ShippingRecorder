using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShippingRecorder.Tests.Config
{
    [TestClass]
    public class ConfigReaderTest
    {
        [TestMethod]
        public void ReadSettingsTest()
        {
            var settings = new ConfigReader<ShippingRecorderApplicationSettings>().Read("appsettings.json");

            Assert.AreEqual("ShippingRecorder.log", settings.LogFile);
            Assert.AreEqual(Severity.Info, settings.MinimumLogLevel);
        }
    }
}

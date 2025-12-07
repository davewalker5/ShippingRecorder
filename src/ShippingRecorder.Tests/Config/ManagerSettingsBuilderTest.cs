using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.Entities.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShippingRecorder.Tests.Config
{
    [TestClass]
    public class ManagerSettingsBuilderTest
    {
        [TestMethod]
        public void BuildSettingsTest()
        {
            var parser = new ManagerCommandLineParser(null);
            var settings = new ManagerSettingsBuilder().BuildSettings(parser, "appsettings.json");

            Assert.AreEqual("ShippingRecorder.log", settings.LogFile);
            Assert.AreEqual(Severity.Info, settings.MinimumLogLevel);
        }
    }
}

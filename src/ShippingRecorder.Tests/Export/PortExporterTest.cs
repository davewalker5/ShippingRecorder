using ShippingRecorder.Data;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Export;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Tests.Mocks;
using Moq;
using ShippingRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Factory;

namespace ShippingRecorder.Tests.Export
{
    [TestClass]
    public class PortExportTest
    {
        private readonly Port _port = DataGenerator.CreatePort();

        private string _filePath;

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_port.Code}"",""{_port.Name}""";
            var exportable = ExportablePort.FromCsv(record);
            Assert.AreEqual(_port.Code, exportable.Code);
            Assert.AreEqual(_port.Name, exportable.Name);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            await context.Countries.AddAsync(_port.Country);
            await context.Ports.AddAsync(_port);
            await context.SaveChangesAsync();

            var factory = new ShippingRecorderFactory(context, new MockFileLogger());
            var exporter = new PortExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var records = File.ReadAllLines(_filePath);
            Assert.HasCount(2, records);

            var exportable = ExportablePort.FromCsv(records[1]);
            Assert.AreEqual(_port.Code, exportable.Code);
            Assert.AreEqual(_port.Name, exportable.Name);
        }
    }
}
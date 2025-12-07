using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Exceptions;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Tests.Mocks;

namespace ShippingRecorder.Tests.Import
{
    [TestClass]
    public class PortImporterTest
    {
        private const string Code ="GBSOU";
        private const string Name = "Southampton";

        private IShippingRecorderFactory _factory;
        private long _countryId;
        private string _filePath;

        [TestInitialize]
        public async Task Initialise()
        {
            ShippingRecorderDbContext context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _countryId  = (await _factory.Countries.AddAsync(Code[..2], "United Kingdom")).Id;
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (!string.IsNullOrEmpty(_filePath) && File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        [TestMethod]
        public async Task ImportTest()
        {
            var record = $@"""{Code}"",""{Name}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new PortImporter(_factory, ExportablePort.CsvRecordPattern);
            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var ports = await _factory.Ports.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, ports);
            Assert.AreEqual(Code, ports.First().Code);
            Assert.AreEqual(Name, ports.First().Name);
        }

        [TestMethod]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new PortImporter(_factory, ExportablePort.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidRecordFormatException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidCodeTest()
        {
            _ = await _factory.Ports.AddAsync(_countryId, Code, Name);
            var record = $@"""{Code}"",""{Name}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new PortImporter(_factory, ExportablePort.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }
    }
}
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
    public class LocationImporterTest
    {
        private const string Name = "Arrecife";

        private IShippingRecorderFactory _factory;
        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            ShippingRecorderDbContext context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
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
            var importer = new LocationImporter(_factory, ExportableLocation.CsvRecordPattern);

            var record = $@"""{Name}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var locations = await _factory.Locations.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, locations);
            Assert.AreEqual(Name, locations.First().Name);
        }

        [TestMethod]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new LocationImporter(_factory, ExportableLocation.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidRecordFormatException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidFieldValueTest()
        {
            const string Name = "Arrecife";
            await _factory.Locations.AddAsync(Name);
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", $@"""{Name}"""]);
            var importer = new LocationImporter(_factory, ExportableLocation.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }
    }
}
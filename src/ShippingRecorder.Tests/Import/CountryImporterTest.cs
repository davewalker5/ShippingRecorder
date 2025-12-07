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
    public class CountryImporterTest
    {
        private const string Code = "GB";
        private const string Name = "United Kingdom";

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
            var importer = new CountryImporter(_factory, ExportableCountry.CsvRecordPattern);

            var record = $@"""{Code}"",""{Name}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var countries = await _factory.Countries.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, countries);
            Assert.AreEqual(Name, countries.First().Name);
        }

        [TestMethod]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new CountryImporter(_factory, ExportableCountry.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidRecordFormatException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidFieldValueTest()
        {
            var record = $@"""{Code}"",""{Name}""";
            await _factory.Countries.AddAsync(Code, Name);
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new CountryImporter(_factory, ExportableCountry.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }
    }
}
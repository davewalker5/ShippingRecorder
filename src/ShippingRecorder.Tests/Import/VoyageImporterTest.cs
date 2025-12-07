using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Exceptions;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Tests.Mocks;

namespace ShippingRecorder.Tests.Import
{
    [TestClass]
    public class VoyageImporterTest
    {
        private const string Operator = "Carnival Uk";
        private const string Number = "K511";
        private const string Port = "GBSOU";

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
            var gb = await _factory.Countries.AddAsync("GB", "United Kingdom");
            _  = await _factory.Ports.AddAsync(gb.Id, Port, "Southampton");
            _  = await _factory.Operators.AddAsync(Operator);

            var date = DateTime.Today;
            var record = $@"""{Operator}"",""{Number}"",""Depart"",""{Port}"",""{date.ToString(ExportableVoyage.DateFormat)}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new VoyageImporter(_factory, ExportableVoyage.CsvRecordPattern);
            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var voyages = await _factory.Voyages.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, voyages);
            Assert.AreEqual(Operator, voyages.First().Operator.Name);
            Assert.AreEqual(Number, voyages.First().Number);
            Assert.AreEqual(VoyageEventType.Depart, voyages.First().Events.First().EventType);
            Assert.AreEqual(Port, voyages.First().Events.First().Port.Code);
            Assert.AreEqual(date, voyages.First().Events.First().Date);
        }

        [TestMethod]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new VoyageImporter(_factory, ExportableVoyage.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidRecordFormatException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidOperatorTest()
        {
            var gb = await _factory.Countries.AddAsync("GB", "United Kingdom");
            _  = await _factory.Ports.AddAsync(gb.Id, Port, "Southampton");

            var date = DateTime.Today;
            var record = $@"""{Operator}"",""{Number}"",""Depart"",""{Port}"",""{date.ToString(ExportableVoyage.DateFormat)}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new VoyageImporter(_factory, ExportableVoyage.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidPortTest()
        {
            _  = await _factory.Operators.AddAsync(Operator);

            var date = DateTime.Today;
            var record = $@"""{Operator}"",""{Number}"",""Depart"",""{Port}"",""{date.ToString(ExportableVoyage.DateFormat)}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new VoyageImporter(_factory, ExportableVoyage.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidEventTypeTest()
        {
            var gb = await _factory.Countries.AddAsync("GB", "United Kingdom");
            _  = await _factory.Ports.AddAsync(gb.Id, Port, "Southampton");
            _  = await _factory.Operators.AddAsync(Operator);

            var date = DateTime.Today;
            var record = $@"""{Operator}"",""{Number}"",""Invalid"",""{Port}"",""{date.ToString(ExportableVoyage.DateFormat)}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new VoyageImporter(_factory, ExportableVoyage.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidDateTest()
        {
            var gb = await _factory.Countries.AddAsync("GB", "United Kingdom");
            _  = await _factory.Ports.AddAsync(gb.Id, Port, "Southampton");
            _  = await _factory.Operators.AddAsync(Operator);

            var date = DateTime.Today;
            var record = $@"""{Operator}"",""{Number}"",""Depart"",""{Port}"",""31-Jun-2025""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new VoyageImporter(_factory, ExportableVoyage.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }
    }
}
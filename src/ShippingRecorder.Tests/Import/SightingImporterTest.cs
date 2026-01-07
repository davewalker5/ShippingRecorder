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
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Tests.Mocks;

namespace ShippingRecorder.Tests.Import
{
    [TestClass]
    public class SightingImporterTest
    {
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
            var date = DateTime.Today;
            var location = DataGenerator.CreateLocation();
            var vessel = DataGenerator.CreateVessel();

            location = await _factory.Locations.AddAsync(location.Name);
            vessel = await _factory.Vessels.AddAsync(vessel.IMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam);

            var importer = new SightingImporter(_factory, ExportableSighting.CsvRecordPattern);

            var record = $@"""{date.ToString(ExportableSighting.DateFormat)}"",""{location.Name}"",""{vessel.IMO}"",""False""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var sightings = await _factory.Sightings.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, sightings);
            Assert.AreEqual(date, sightings.First().Date);
            Assert.AreEqual(location.Id, sightings.First().LocationId);
            Assert.AreEqual(vessel.Id, sightings.First().VesselId);
            Assert.IsFalse(sightings.First().IsMyVoyage);
        }

        [TestMethod]
        public async Task ImportForMissingLocationTest()
        {
            var date = DateTime.Today;
            var vessel = DataGenerator.CreateVessel();

            vessel = await _factory.Vessels.AddAsync(vessel.IMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam);

            var importer = new SightingImporter(_factory, ExportableSighting.CsvRecordPattern);

            var record = $@"""{date.ToString(ExportableSighting.DateFormat)}"",""Missing"",""{vessel.IMO}"",""False""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task ImportForMissingVesselTest()
        {
            var date = DateTime.Today;
            var location = DataGenerator.CreateLocation();

            location = await _factory.Locations.AddAsync(location.Name);

            var importer = new SightingImporter(_factory, ExportableSighting.CsvRecordPattern);

            var record = $@"""{date.ToString(ExportableSighting.DateFormat)}"",""{location.Name}"",""1234567"",""False""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task ImportForFutureDateTest()
        {
            var date = DateTime.Today.AddDays(2);
            var location = DataGenerator.CreateLocation();
            var vessel = DataGenerator.CreateVessel();

            location = await _factory.Locations.AddAsync(location.Name);
            vessel = await _factory.Vessels.AddAsync(vessel.IMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam);

            var importer = new SightingImporter(_factory, ExportableSighting.CsvRecordPattern);

            var record = $@"""{date.ToString(ExportableSighting.DateFormat)}"",""{location.Name}"",""{vessel.IMO}"",""False""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new SightingImporter(_factory, ExportableLocation.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidRecordFormatException>(() => importer.ImportAsync(_filePath));
        }
    }
}
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
    public class VesselImporterTest
    {
        private const string VesselIdentifier = "8420878";
        private const int Built = 2005;
        private const int Length = 285;
        private const int Beam = 32;
        private const decimal Draught = 8.1M;
        private const int Tonnage = 84342;
        private const int Passengers = 2094;
        private const int Crew = 866;
        private const int Decks = 11;
        private const int Cabins = 1050;
        private const string Name = "Arcadia";
        private const string Callsign ="ZCDN2";
        private const string MMSI ="310459000";
        private const string Flag = "BM";
        private const string Type = "Passenger (Cruise) Ship";
        private const string Operator = "Carnival UK";

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
            _  = await _factory.Countries.AddAsync(Flag, "Bermuda");
            _  = await _factory.VesselTypes.AddAsync(Type);
            _  = await _factory.Operators.AddAsync(Operator);

            var record = $@"""{VesselIdentifier}"",""True"",""{Built}"",""{Draught}"",""{Length}"",""{Beam}"",""{Tonnage}"",""{Passengers}"",""{Crew}"",""{Decks}"",""{Cabins}"",""{Name}"",""{Callsign}"",""{MMSI}"",""{Type}"",""{Flag}"",""{Operator}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);

            var importer = new VesselImporter(_factory, ExportableVessel.CsvRecordPattern);
            await importer.ImportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var vessels = await _factory.Vessels.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, vessels);
            Assert.AreEqual(VesselIdentifier, vessels.First().Identifier);
        }

        [TestMethod]
        public async Task InvalidRecordFormatTest()
        {
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", "Invalid record format"]);
            var importer = new VesselImporter(_factory, ExportableVessel.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidRecordFormatException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidIdentifierTest()
        {
            _  = await _factory.Countries.AddAsync(Flag, "Bermuda");
            _  = await _factory.VesselTypes.AddAsync(Type);
            _  = await _factory.Operators.AddAsync(Operator);
            _ = await _factory.Vessels.AddAsync(VesselIdentifier, true, Built, Draught, Length, Beam);

            var record = $@"""{VesselIdentifier}"",""True"",""{Built}"",""{Draught}"",""{Length}"",""{Beam}"",""{Tonnage}"",""{Passengers}"",""{Crew}"",""{Decks}"",""{Cabins}"",""{Name}"",""{Callsign}"",""{MMSI}"",""{Type}"",""{Flag}"",""{Operator}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new VesselImporter(_factory, ExportableVessel.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidVesselTypeTest()
        {
            _  = await _factory.Countries.AddAsync(Flag, "Bermuda");
            _  = await _factory.Operators.AddAsync(Operator);

            var record = $@"""{VesselIdentifier}"",""True"",""{Built}"",""{Draught}"",""{Length}"",""{Beam}"",""{Tonnage}"",""{Passengers}"",""{Crew}"",""{Decks}"",""{Cabins}"",""{Name}"",""{Callsign}"",""{MMSI}"",""{Type}"",""{Flag}"",""{Operator}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new VesselImporter(_factory, ExportableVessel.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidFlagTest()
        {
            _  = await _factory.VesselTypes.AddAsync(Type);
            _  = await _factory.Operators.AddAsync(Operator);

            var record = $@"""{VesselIdentifier}"",""True"",""{Built}"",""{Draught}"",""{Length}"",""{Beam}"",""{Tonnage}"",""{Passengers}"",""{Crew}"",""{Decks}"",""{Cabins}"",""{Name}"",""{Callsign}"",""{MMSI}"",""{Type}"",""{Flag}"",""{Operator}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new VesselImporter(_factory, ExportableVessel.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }

        [TestMethod]
        public async Task InvalidOperatorTest()
        {
            _  = await _factory.Countries.AddAsync(Flag, "Bermuda");
            _  = await _factory.VesselTypes.AddAsync(Type);

            var record = $@"""{VesselIdentifier}"",""True"",""{Built}"",""{Draught}"",""{Length}"",""{Beam}"",""{Tonnage}"",""{Passengers}"",""{Crew}"",""{Decks}"",""{Cabins}"",""{Name}"",""{Callsign}"",""{MMSI}"",""{Type}"",""{Flag}"",""{Operator}""";
            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            File.WriteAllLines(_filePath, ["", record]);
            var importer = new VesselImporter(_factory, ExportableVessel.CsvRecordPattern);
            await Assert.ThrowsAsync<InvalidFieldValueException>(() => importer.ImportAsync(_filePath));
        }
    }
}
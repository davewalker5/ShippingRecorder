using ShippingRecorder.Data;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Export;
using ShippingRecorder.DataExchange.Extensions;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Tests.Mocks;
using Moq;
using ShippingRecorder.Entities.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Factory;

namespace ShippingRecorder.Tests.Export
{
    [TestClass]
    public class VesselExportTest
    {
        private readonly Vessel _vessel = DataGenerator.CreateVessel();

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
        public void ConvertSingleObjectToExportable()
        {
            var exportable = _vessel.ToExportable();
            Assert.AreEqual(_vessel.IMO, exportable.IMO);
            Assert.AreEqual(_vessel.Built, exportable.Built);
            Assert.AreEqual(_vessel.Draught, exportable.Draught);
            Assert.AreEqual(_vessel.Length, exportable.Length);
            Assert.AreEqual(_vessel.Beam, exportable.Beam);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.VesselType.Name, exportable.VesselType);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Flag.Code, exportable.Flag);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Operator.Name, exportable.Operator);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Name, exportable.Name);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Callsign, exportable.Callsign);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.MMSI, exportable.MMSI);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Tonnage, exportable.Tonnage);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Passengers, exportable.Passengers);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Crew, exportable.Crew);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Decks, exportable.Decks);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Cabins, exportable.Cabins);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<Vessel> vessels = [_vessel];
            var exportable = vessels.ToExportable();
            Assert.AreEqual(_vessel.IMO, exportable.First().IMO);
            Assert.AreEqual(_vessel.Built, exportable.First().Built);
            Assert.AreEqual(_vessel.Draught, exportable.First().Draught);
            Assert.AreEqual(_vessel.Length, exportable.First().Length);
            Assert.AreEqual(_vessel.Beam, exportable.First().Beam);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.VesselType.Name, exportable.First().VesselType);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Flag.Code, exportable.First().Flag);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Operator.Name, exportable.First().Operator);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Name, exportable.First().Name);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Callsign, exportable.First().Callsign);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.MMSI, exportable.First().MMSI);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Tonnage, exportable.First().Tonnage);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Passengers, exportable.First().Passengers);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Crew, exportable.First().Crew);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Decks, exportable.First().Decks);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Cabins, exportable.First().Cabins);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_vessel.IMO}"",""{_vessel.Built}"",""{_vessel.Draught}"",""{_vessel.Length}"",""{_vessel.Beam}"",""{_vessel.ActiveRegistrationHistory.Tonnage}"",""{_vessel.ActiveRegistrationHistory.Passengers}"",""{_vessel.ActiveRegistrationHistory.Crew}"",""{_vessel.ActiveRegistrationHistory.Decks}"",""{_vessel.ActiveRegistrationHistory.Cabins}"",""{_vessel.ActiveRegistrationHistory.Name}"",""{_vessel.ActiveRegistrationHistory.Callsign}"",""{_vessel.ActiveRegistrationHistory.MMSI}"",""{_vessel.ActiveRegistrationHistory.VesselType.Name}"",""{_vessel.ActiveRegistrationHistory.Flag.Code}"",""{_vessel.ActiveRegistrationHistory.Operator.Name}""";
            var exportable = ExportableVessel.FromCsv(record);
            Assert.AreEqual(_vessel.IMO, exportable.IMO);
            Assert.AreEqual(_vessel.Built, exportable.Built);
            Assert.AreEqual(_vessel.Draught, exportable.Draught);
            Assert.AreEqual(_vessel.Length, exportable.Length);
            Assert.AreEqual(_vessel.Beam, exportable.Beam);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.VesselType.Name, exportable.VesselType);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Flag.Code, exportable.Flag);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Operator.Name, exportable.Operator);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Name, exportable.Name);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Callsign, exportable.Callsign);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.MMSI, exportable.MMSI);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Tonnage, exportable.Tonnage);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Passengers, exportable.Passengers);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Crew, exportable.Crew);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Decks, exportable.Decks);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Cabins, exportable.Cabins);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            await context.VesselTypes.AddAsync(_vessel.ActiveRegistrationHistory.VesselType);
            await context.Countries.AddAsync(_vessel.ActiveRegistrationHistory.Flag);
            await context.Operators.AddAsync(_vessel.ActiveRegistrationHistory.Operator);
            await context.RegistrationHistory.AddAsync(_vessel.ActiveRegistrationHistory);
            await context.SaveChangesAsync();
            await context.Vessels.AddAsync(_vessel);
            await context.SaveChangesAsync();

            var logger = new Mock<IShippingRecorderLogger>();
            var factory = new ShippingRecorderFactory(context, new MockFileLogger());
            var exporter = new VesselExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<Vessel> measurements = [_vessel];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var records = File.ReadAllLines(_filePath);
            Assert.HasCount(2, records);

            var exportable = ExportableVessel.FromCsv(records[1]);
            Assert.AreEqual(_vessel.IMO, exportable.IMO);
            Assert.AreEqual(_vessel.Built, exportable.Built);
            Assert.AreEqual(_vessel.Draught, exportable.Draught);
            Assert.AreEqual(_vessel.Length, exportable.Length);
            Assert.AreEqual(_vessel.Beam, exportable.Beam);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.VesselType.Name, exportable.VesselType);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Flag.Code, exportable.Flag);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Operator.Name, exportable.Operator);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Name, exportable.Name);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Callsign, exportable.Callsign);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.MMSI, exportable.MMSI);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Tonnage, exportable.Tonnage);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Passengers, exportable.Passengers);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Crew, exportable.Crew);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Decks, exportable.Decks);
            Assert.AreEqual(_vessel.ActiveRegistrationHistory.Cabins, exportable.Cabins);
        }
    }
}
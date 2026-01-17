using ShippingRecorder.Data;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Export;
using ShippingRecorder.DataExchange.Extensions;
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
    public class VoyageExportTest
    {
        private Voyage _voyage;

        private string _filePath;

        [TestInitialize]
        public void Initialise()
        {
            _voyage = DataGenerator.CreateVoyage();
            _voyage.Events = [DataGenerator.CreateVoyageEvent(_voyage.Id)];
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
        public void ConvertSingleObjectToExportable()
        {
            var exportable = _voyage.ToExportable();
            Assert.AreEqual(_voyage.Vessel.IMO, exportable.First().IMO);
            Assert.AreEqual(_voyage.Operator.Name, exportable.First().Operator);
            Assert.AreEqual(_voyage.Number, exportable.First().Number);
            Assert.AreEqual(_voyage.Events.First().EventType.ToString(), exportable.First().EventType);
            Assert.AreEqual(_voyage.Events.First().Port.Code, exportable.First().Port);
            Assert.AreEqual(_voyage.Events.First().Date.ToString(ExportableEntityBase.DateFormat), exportable.First().Date);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<Voyage> vessels = [_voyage];
            var exportable = vessels.ToExportable();
            Assert.AreEqual(_voyage.Vessel.IMO, exportable.First().IMO);
            Assert.AreEqual(_voyage.Operator.Name, exportable.First().Operator);
            Assert.AreEqual(_voyage.Number, exportable.First().Number);
            Assert.AreEqual(_voyage.Events.First().EventType.ToString(), exportable.First().EventType);
            Assert.AreEqual(_voyage.Events.First().Port.Code, exportable.First().Port);
            Assert.AreEqual(_voyage.Events.First().Date.ToString(ExportableEntityBase.DateFormat), exportable.First().Date);

        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var record = $@"""{_voyage.Vessel.IMO}"",""{_voyage.Operator.Name}"",""{_voyage.Number}"",""{_voyage.Events.First().EventType}"",""{_voyage.Events.First().Port.Code}"",""{_voyage.Events.First().Date.ToString(ExportableVoyage.DateFormat)}""";
            var exportable = ExportableVoyage.FromCsv(record);
            Assert.AreEqual(_voyage.Vessel.IMO, exportable.IMO);
            Assert.AreEqual(_voyage.Operator.Name, exportable.Operator);
            Assert.AreEqual(_voyage.Number, exportable.Number);
            Assert.AreEqual(_voyage.Events.First().EventType.ToString(), exportable.EventType);
            Assert.AreEqual(_voyage.Events.First().Port.Code, exportable.Port);
            Assert.AreEqual(_voyage.Events.First().Date.ToString(ExportableEntityBase.DateFormat), exportable.Date);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            await context.Vessels.AddAsync(_voyage.Vessel);
            await context.Operators.AddAsync(_voyage.Operator);
            await context.Countries.AddAsync(_voyage.Events.First().Port.Country);
            await context.Ports.AddAsync(_voyage.Events.First().Port);
            await context.SaveChangesAsync();
            await context.Voyages.AddAsync(_voyage);
            await context.SaveChangesAsync();

            var factory = new ShippingRecorderFactory(context, new MockFileLogger());
            var exporter = new VoyageExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var records = File.ReadAllLines(_filePath);
            Assert.HasCount(2, records);

            var exportable = ExportableVoyage.FromCsv(records[1]);
            Assert.AreEqual(_voyage.Vessel.IMO, exportable.IMO);
            Assert.AreEqual(_voyage.Operator.Name, exportable.Operator);
            Assert.AreEqual(_voyage.Number, exportable.Number);
            Assert.AreEqual(_voyage.Events.First().EventType.ToString(), exportable.EventType);
            Assert.AreEqual(_voyage.Events.First().Port.Code, exportable.Port);
            Assert.AreEqual(_voyage.Events.First().Date.ToString(ExportableEntityBase.DateFormat), exportable.Date);
        }
    }
}
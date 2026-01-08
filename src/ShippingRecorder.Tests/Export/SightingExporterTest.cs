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
    public class SightingExportTest
    {
        private readonly Sighting _sighting = DataGenerator.CreateSighting();

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
            var exportable = _sighting.ToExportable();
            Assert.AreEqual(_sighting.Date, exportable.Date);
            Assert.AreEqual(_sighting.Location.Name, exportable.Location);
            Assert.AreEqual(_sighting.Vessel.IMO, exportable.IMO);
            Assert.AreEqual(_sighting.IsMyVoyage, exportable.IsMyVoyage);
        }

        [TestMethod]
        public void ConvertCollectionToExportable()
        {
            List<Sighting> sightings = [_sighting];
            var exportable = sightings.ToExportable();
            Assert.AreEqual(_sighting.Date, exportable.First().Date);
            Assert.AreEqual(_sighting.Location.Name, exportable.First().Location);
            Assert.AreEqual(_sighting.Vessel.IMO, exportable.First().IMO);
            Assert.AreEqual(_sighting.IsMyVoyage, exportable.First().IsMyVoyage);
        }

        [TestMethod]
        public void FromCsvRecordTest()
        {
            var isMyVoyage = _sighting.IsMyVoyage ? "True" : "False";
            var record = $@"""{_sighting.Date.ToString(ExportableEntityBase.DateFormat)}"",""{_sighting.Location.Name}"",""{_sighting.Vessel.IMO}"",""{isMyVoyage}""";
            var exportable = ExportableSighting.FromCsv(record);
            Assert.AreEqual(_sighting.Date, exportable.Date);
            Assert.AreEqual(_sighting.Location.Name, exportable.Location);
            Assert.AreEqual(_sighting.Vessel.IMO, exportable.IMO);
            Assert.AreEqual(_sighting.IsMyVoyage, exportable.IsMyVoyage);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            await context.Locations.AddAsync(_sighting.Location);
            await context.Vessels.AddAsync(_sighting.Vessel);
            await context.SaveChangesAsync();
            await context.Sightings.AddAsync(_sighting);
            await context.SaveChangesAsync();


            var logger = new Mock<IShippingRecorderLogger>();
            var factory = new ShippingRecorderFactory(context, new MockFileLogger());
            var exporter = new SightingExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            List<Sighting> measurements = [_sighting];
            await exporter.ExportAsync(measurements, _filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var records = File.ReadAllLines(_filePath);
            Assert.HasCount(2, records);

            var exportable = ExportableSighting.FromCsv(records[1]);
            Assert.AreEqual(_sighting.Date, exportable.Date);
            Assert.AreEqual(_sighting.Location.Name, exportable.Location);
            Assert.AreEqual(_sighting.Vessel.IMO, exportable.IMO);
            Assert.AreEqual(_sighting.IsMyVoyage, exportable.IsMyVoyage);
        }
    }
}
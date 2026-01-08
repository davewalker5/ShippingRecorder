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
    public class OperatorExportTest
    {
        private readonly Operator _location = DataGenerator.CreateOperator();

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
            var record = $@"""{_location.Name}""";
            var exportable = ExportableOperator.FromCsv(record);
            Assert.AreEqual(_location.Name, exportable.Name);
        }

        [TestMethod]
        public async Task ExportTest()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            await context.Operators.AddAsync(_location);
            await context.SaveChangesAsync();

            var logger = new Mock<IShippingRecorderLogger>();
            var factory = new ShippingRecorderFactory(context, new MockFileLogger());
            var exporter = new OperatorExporter(factory);

            _filePath = Path.ChangeExtension(Path.GetTempFileName(), "csv");
            await exporter.ExportAsync(_filePath);

            var info = new FileInfo(_filePath);
            Assert.AreEqual(info.FullName, _filePath);
            Assert.IsGreaterThan(0, info.Length);

            var records = File.ReadAllLines(_filePath);
            Assert.HasCount(2, records);

            var exportable = ExportableOperator.FromCsv(records[1]);
            Assert.AreEqual(_location.Name, exportable.Name);
        }
    }
}
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Exceptions;

namespace ShippingRecorder.Tests
{
    [TestClass]
    public class JobStatusManagerTest
    {
        private const string Name = "SightingsExport";
        private const string Parameters = "2026-01-07 Export.csv";
        private const string Error = "Some error message";

        private ShippingRecorderFactory _factory;
        private long _statusId;

        [TestInitialize]
        public async Task TestInitialize()
        {
            ShippingRecorderDbContext context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _statusId = (await _factory.JobStatuses.AddAsync(Name, Parameters)).Id;
        }

        [TestMethod]
        public async Task AddAndGetAsyncTest()
        {
            var status = await _factory.JobStatuses.GetAsync(x => x.Id == _statusId);

            Assert.IsNotNull(status);
            Assert.AreEqual(_statusId, status.Id);
            Assert.AreEqual(Name, status.Name);
            Assert.AreEqual(Parameters, status.Parameters);
            Assert.IsNull(status.End);
            Assert.IsNull(status.Error);
        }

        [TestMethod]
        public async Task ListAsyncTest()
        {
            var statuses = await _factory.JobStatuses.ListAsync(x => true, 1, 10).ToListAsync();

            Assert.IsNotNull(statuses);
            Assert.HasCount(1, statuses);
            Assert.AreEqual(_statusId, statuses[0].Id);
            Assert.AreEqual(Name, statuses[0].Name);
            Assert.AreEqual(Parameters, statuses[0].Parameters);
            Assert.IsNull(statuses[0].End);
            Assert.IsNull(statuses[0].Error);
        }

        [TestMethod]
        public async Task ListAllAsyncTest()
        {
            var statuses = await _factory.JobStatuses.ListAsync(null, 1, 10).ToListAsync();

            Assert.IsNotNull(statuses);
            Assert.HasCount(1, statuses);
            Assert.AreEqual(_statusId, statuses[0].Id);
            Assert.AreEqual(Name, statuses[0].Name);
            Assert.AreEqual(Parameters, statuses[0].Parameters);
            Assert.IsNull(statuses[0].End);
            Assert.IsNull(statuses[0].Error);
        }

        [TestMethod]
        public async Task UpdateAsyncTest()
        {
            var status = await _factory.JobStatuses.UpdateAsync(_statusId, Error);

            Assert.IsNotNull(status);
            Assert.AreEqual(_statusId, status.Id);
            Assert.AreEqual(Name, status.Name);
            Assert.AreEqual(Parameters, status.Parameters);
            Assert.IsNotNull(status.End);
            Assert.AreEqual(Error, status.Error);
        }

        [TestMethod]
        public async Task UpdateMissingAsyncTest()
            => await Assert.ThrowsAsync<JobStatusNotFoundException>(() => _factory.JobStatuses.UpdateAsync(-1, Error));
    }
}

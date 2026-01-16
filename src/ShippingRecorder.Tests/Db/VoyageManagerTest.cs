using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.Entities.Db;
using System;

namespace ShippingRecorder.Tests.Db
{
    [TestClass]
    public class VoyageManagerTest
    {
        private long _operatorId;
        private long _vesselId;
        private long _secondOperatorId;
        private long _secondVesselId;
        private const string Number = "9272QIU261";
        private const string SecondNumber = "039271HGFU";

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _operatorId = (await _factory.Operators.AddAsync("P&O Ferries")).Id;
            _vesselId = (await _factory.Vessels.AddAsync("9226906", null, null, null, null)).Id;
            _secondOperatorId = (await _factory.Operators.AddAsync("Royal Caribbean International")).Id;
            _secondVesselId = (await _factory.Vessels.AddAsync("9744001", null, null, null, null)).Id;
            _ = await _factory.Voyages.AddAsync(_operatorId, _vesselId, Number);
        }

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Voyages.GetAsync(e => (e.OperatorId == _operatorId) && (e.Number == Number));
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(_operatorId, entity.OperatorId);
            Assert.AreEqual(_vesselId, entity.VesselId);
            Assert.AreEqual(Number, entity.Number);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.Voyages.GetAsync(e => (e.OperatorId == _operatorId) && (e.Number == "Missing"));
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Voyages.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(_operatorId, entities.First().OperatorId);
            Assert.AreEqual(_vesselId, entities.First().VesselId);
            Assert.AreEqual(Number, entities.First().Number);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Voyages.ListAsync(e => (e.OperatorId == _operatorId) && (e.Number == Number), 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(_operatorId, entities.First().OperatorId);
            Assert.AreEqual(_vesselId, entities.First().VesselId);
            Assert.AreEqual(Number, entities.First().Number);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Voyages.ListAsync(e => (e.OperatorId == _operatorId) && (e.Number == "Missing"), 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Voyages.GetAsync(e => (e.OperatorId == _operatorId) && (e.Number == Number));
            var updated = await _factory.Voyages.UpdateAsync(entity.Id, _secondOperatorId, _secondVesselId, SecondNumber);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(_secondOperatorId, entity.OperatorId);
            Assert.AreEqual(_secondVesselId, entity.VesselId);
            Assert.AreEqual(SecondNumber, entity.Number);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<VoyageNotFoundException>(() => _factory.Voyages.UpdateAsync(-1, _secondOperatorId, _secondVesselId, SecondNumber));

        [TestMethod]
        public async Task DeleteWithNoEventsTestAsync()
        {
            var entity = await _factory.Voyages.GetAsync(e => (e.OperatorId == _operatorId) && (e.Number == Number));
            await _factory.Voyages.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Sightings.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteWithEventsTestAsync()
        {
            var entity = await _factory.Voyages.GetAsync(e => (e.OperatorId == _operatorId) && (e.Number == Number));
            var country = await _factory.Countries.AddAsync("GB", "United Kingdom");
            var port = await _factory.Ports.AddAsync(country.Id, "GBSOU", "Sounthampton");
            _ = await _factory.VoyageEvents.AddAsync(entity.Id, port.Id, VoyageEventType.Depart, DateTime.Today);

            await _factory.Voyages.DeleteAsync(entity.Id);

            var count = _factory.GetContext<ShippingRecorderDbContext>().Voyages.Count();
            Assert.AreEqual(0, count);

            count = _factory.GetContext<ShippingRecorderDbContext>().VoyageEvents.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<VoyageNotFoundException>(() => _factory.Voyages.DeleteAsync(-1));
    }
}

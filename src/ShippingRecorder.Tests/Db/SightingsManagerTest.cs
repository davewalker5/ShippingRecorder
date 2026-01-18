using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Tests.Mocks;

namespace ShippingRecorder.Tests.Db
{
    [TestClass]
    public class SightingsManagerTest
    {
        private long _operatorId;
        private long _voyageId;
        private long _vesselTypeId;
        private long _vesselId;
        private long _locationId;
        private readonly DateTime Date = DateTime.Now;

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _operatorId = (await _factory.Operators.AddAsync("P&O Ferries")).Id;
            _vesselTypeId = (await _factory.VesselTypes.AddAsync("Passenger Ship")).Id;
            _vesselId = (await _factory.Vessels.AddAsync("8420878", true, null, null, null, null)).Id;
            _voyageId = (await _factory.Voyages.AddAsync(_operatorId, _vesselId, "9272QIU261")).Id;
            _locationId = (await _factory.Locations.AddAsync("Arrecife")).Id;
            _ = await _factory.Sightings.AddAsync(_locationId, _voyageId, _vesselId, Date, false);
        }

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Sightings.GetAsync(e => true);
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(_locationId, entity.LocationId);
            Assert.AreEqual(_voyageId, entity.VoyageId);
            Assert.AreEqual(_vesselId, entity.VesselId);
            Assert.AreEqual(Date, entity.Date);
            Assert.IsFalse(entity.IsMyVoyage);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Sightings.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.IsGreaterThan(0, entities.First().Id);
            Assert.AreEqual(_locationId, entities.First().LocationId);
            Assert.AreEqual(_voyageId, entities.First().VoyageId);
            Assert.AreEqual(_vesselId, entities.First().VesselId);
            Assert.AreEqual(Date, entities.First().Date);
            Assert.IsFalse(entities.First().IsMyVoyage);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Sightings.ListAsync(e => e.VesselId == _vesselId, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.IsGreaterThan(0, entities.First().Id);
            Assert.AreEqual(_locationId, entities.First().LocationId);
            Assert.AreEqual(_voyageId, entities.First().VoyageId);
            Assert.AreEqual(_vesselId, entities.First().VesselId);
            Assert.AreEqual(Date, entities.First().Date);
            Assert.IsFalse(entities.First().IsMyVoyage);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Sightings.ListAsync(e => e.VesselId == -1, 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Sightings.GetAsync(e => true);
            var updated = await _factory.Sightings.UpdateAsync(entity.Id, _locationId, _voyageId, _vesselId, Date.AddDays(1), true);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(_locationId, entity.LocationId);
            Assert.AreEqual(_voyageId, entity.VoyageId);
            Assert.AreEqual(_vesselId, entity.VesselId);
            Assert.AreEqual(Date.AddDays(1), entity.Date);
            Assert.IsTrue(entity.IsMyVoyage);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<SightingNotFoundException>(() => _factory.Sightings.UpdateAsync(-1, _locationId, _voyageId, _vesselId, Date.AddDays(1), true));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await _factory.Sightings.GetAsync(e => true);
            await _factory.Sightings.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Sightings.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<SightingNotFoundException>(() => _factory.Sightings.DeleteAsync(-1));
    }
}
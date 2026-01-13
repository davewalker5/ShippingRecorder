using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ShippingRecorder.Entities.Db;
using System;

namespace ShippingRecorder.Tests.Db
{
    [TestClass]
    public class VoyageEventManagerTest
    {
        private const string OperatorName = "P&O Ferries";
        private const string IMO = "9826548";
        private const string VoyageNumber = "87236JGH78";

        private readonly List<dynamic> VoyageEvents =
        [
            new { Date = new DateTime(2025, 11, 17, 0, 0, 0), EventType = VoyageEventType.Depart, CountryCode = "GB", Country = "United Kingdom", PortCode = "GBSOU", PortName = "Southampton" },
            new { Date = new DateTime(2025, 11, 24, 0, 0, 0), EventType = VoyageEventType.Arrive, CountryCode = "ES", Country = "Spain", PortCode = "ESSCT", PortName = "Santa Cruz de Tenerife" },
            new { Date = new DateTime(2025, 11, 25, 0, 0, 0), EventType = VoyageEventType.Depart, CountryCode = "ES", Country = "Spain", PortCode = "ESSCT", PortName = "Santa Cruz de Tenerife" },
            new { Date = new DateTime(2025, 12, 3, 0, 0, 0), EventType = VoyageEventType.Arrive, CountryCode = "GB", Country = "United Kingdom", PortCode = "GBSOU", PortName = "Southampton" }
        ];

        private readonly List<Port> Ports = [];

        private ShippingRecorderFactory _factory;
        private Voyage _voyage = null;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());

            // Create the countries and ports
            foreach (var voyageEvent in VoyageEvents)
            {
                var country = await _factory.Countries.AddIfNotExistsAsync(voyageEvent.CountryCode, voyageEvent.Country);
                var port = await _factory.Ports.AddIfNotExistsAsync(country.Id, voyageEvent.PortCode, voyageEvent.PortName);
                Ports.Add(port);
            }

            // Create the voyage
            var op = await _factory.Operators.AddAsync(OperatorName);
            var vessel = await _factory.Vessels.AddAsync(IMO, null, null, null, null);
            _voyage = await _factory.Voyages.AddAsync(op.Id, vessel.Id, VoyageNumber);

            // Create the voyage events
            for (int i = 0; i < VoyageEvents.Count; i++)
            {
                _ = await _factory.VoyageEvents.AddAsync(_voyage.Id, Ports[i].Id, VoyageEvents[i].EventType, VoyageEvents[i].Date);
            }
        }

        private async Task<VoyageEvent> GetFirstEvent()
            => await _factory.VoyageEvents.GetAsync(e => (e.VoyageId == _voyage.Id) && (e.PortId == Ports[0].Id) && (e.EventType == VoyageEventType.Depart));

        [TestMethod]
        public async Task CannotAddDuplicateTestAsync()
            => await Assert.ThrowsAsync<VoyageEventExistsException>(() => _factory.VoyageEvents.AddAsync(_voyage.Id, Ports[0].Id, VoyageEvents[0].EventType, VoyageEvents[0].Date));

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await GetFirstEvent();
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(_voyage.Id, entity.VoyageId);
            Assert.AreEqual(Ports[0].Id, entity.PortId);
            Assert.AreEqual(VoyageEvents[0].EventType, entity.EventType);
            Assert.AreEqual(VoyageEvents[0].Date, entity.Date);
        }

        [TestMethod]
        public async Task AddExistingIfNotExistTestAsync()
        {
            _ = await _factory.VoyageEvents.AddIfNotExistsAsync(_voyage.Id, Ports[0].Id, VoyageEvents[0].EventType, VoyageEvents[0].Date);
            var entities = await _factory.VoyageEvents.ListAsync(x => x.VoyageId == _voyage.Id, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(4, entities);
        }

        [TestMethod]
        public async Task AddMissingIfNotExistTestAsync()
        {
            var entity = await _factory.VoyageEvents.AddIfNotExistsAsync(_voyage.Id, Ports[0].Id, VoyageEventType.Depart, VoyageEvents[0].Date.AddDays(1));
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(_voyage.Id, entity.VoyageId);
            Assert.AreEqual(Ports[0].Id, entity.PortId);
            Assert.AreEqual(VoyageEventType.Depart, entity.EventType);
            Assert.AreEqual(VoyageEvents[0].Date.AddDays(1), entity.Date);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.VoyageEvents.GetAsync(e => e.VoyageId == -1);
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.VoyageEvents.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(4, entities);
            for (int i = 0; i < entities.Count; i++)
            {
                Assert.IsGreaterThan(0, entities[i].Id);
                Assert.AreEqual(_voyage.Id, entities[i].VoyageId);
                Assert.AreEqual(Ports[i].Id, entities[i].PortId);
                Assert.AreEqual(VoyageEvents[i].EventType, entities[i].EventType);
                Assert.AreEqual(VoyageEvents[i].Date, entities[i].Date);
            }
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.VoyageEvents.ListAsync(e => e.VoyageId == _voyage.Id, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(4, entities);
            for (int i = 0; i < entities.Count; i++)
            {
                Assert.IsGreaterThan(0, entities[i].Id);
                Assert.AreEqual(_voyage.Id, entities[i].VoyageId);
                Assert.AreEqual(Ports[i].Id, entities[i].PortId);
                Assert.AreEqual(VoyageEvents[i].EventType, entities[i].EventType);
                Assert.AreEqual(VoyageEvents[i].Date, entities[i].Date);
            }
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.VoyageEvents.ListAsync(e => e.VoyageId == -1, 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await GetFirstEvent();
            var updated = await _factory.VoyageEvents.UpdateAsync(entity.Id, _voyage.Id, Ports[1].Id, VoyageEvents[1].EventType, VoyageEvents[1].Date.AddDays(1));
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(_voyage.Id, entity.VoyageId);
            Assert.AreEqual(Ports[1].Id, entity.PortId);
            Assert.AreEqual(VoyageEvents[1].EventType, entity.EventType);
            Assert.AreEqual(VoyageEvents[1].Date.AddDays(1), entity.Date);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<VoyageEventNotFoundException>(() => _factory.VoyageEvents.UpdateAsync(-1, _voyage.Id, Ports[1].Id, VoyageEvents[1].EventType, VoyageEvents[1].Date));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await GetFirstEvent();
            await _factory.VoyageEvents.DeleteAsync(entity.Id);
            var entities = await _factory.VoyageEvents.ListAsync(e => e.VoyageId == _voyage.Id, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(3, entities);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<VoyageEventNotFoundException>(() => _factory.VoyageEvents.DeleteAsync(-1));

    }
}

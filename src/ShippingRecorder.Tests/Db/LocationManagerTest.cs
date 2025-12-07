using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ShippingRecorder.Tests.Db
{
    [TestClass]
    public class LocationManagerTest
    {
        private const string Name = "Arrecife";
        private const string SecondName = "Southampton";

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _ = await _factory.Locations.AddAsync(Name);
        }

        [TestMethod]
        public async Task CannotAddDuplicateTestAsync()
            => await Assert.ThrowsAsync<LocationExistsException>(() => _factory.Locations.AddAsync(Name));

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Locations.GetAsync(e => e.Name == Name);
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(Name, entity.Name);
        }

        [TestMethod]
        public async Task AddExistingIfNotExistTestAsync()
        {
            _ = await _factory.Locations.AddIfNotExistsAsync(Name);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Locations.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddMissingIfNotExistTestAsync()
        {
            var entity = await _factory.Locations.AddIfNotExistsAsync(SecondName);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Locations.Count();
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(SecondName, entity.Name);
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.Locations.GetAsync(e => e.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Locations.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Locations.ListAsync(e => e.Name == Name, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Locations.ListAsync(e => e.Name == "Missing", 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Locations.GetAsync(e => e.Name == Name);
            var updated = await _factory.Locations.UpdateAsync(entity.Id, SecondName);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(SecondName, entity.Name);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<LocationNotFoundException>(() => _factory.Locations.UpdateAsync(-1, SecondName));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await _factory.Locations.GetAsync(e => e.Name == Name);
            await _factory.Locations.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Locations.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<LocationNotFoundException>(() => _factory.Locations.DeleteAsync(-1));

    }
}

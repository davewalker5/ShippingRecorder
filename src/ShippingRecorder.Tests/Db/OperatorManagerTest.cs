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
    public class OperatorManagerTest
    {
        private const string Name = "P&O Ferries";
        private const string SecondName = "Royal Caribbean International";

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _ = await _factory.Operators.AddAsync(Name);
        }

        [TestMethod]
        public async Task CannotAddDuplicateTestAsync()
            => await Assert.ThrowsAsync<OperatorExistsException>(() => _factory.Operators.AddAsync(Name));

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Operators.GetAsync(e => e.Name == Name);
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(Name, entity.Name);
        }

        [TestMethod]
        public async Task AddExistingIfNotExistTestAsync()
        {
            _ = await _factory.Operators.AddIfNotExistsAsync(Name);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Operators.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddMissingIfNotExistTestAsync()
        {
            var entity = await _factory.Operators.AddIfNotExistsAsync(SecondName);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Operators.Count();
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(SecondName, entity.Name);
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.Operators.GetAsync(e => e.Name == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Operators.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Operators.ListAsync(e => e.Name == Name, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Operators.ListAsync(e => e.Name == "Missing", 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Operators.GetAsync(e => e.Name == Name);
            var updated = await _factory.Operators.UpdateAsync(entity.Id, SecondName);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(SecondName, entity.Name);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<OperatorNotFoundException>(() => _factory.Operators.UpdateAsync(-1, SecondName));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await _factory.Operators.GetAsync(e => e.Name == Name);
            await _factory.Operators.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Operators.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<OperatorNotFoundException>(() => _factory.Operators.DeleteAsync(-1));

    }
}

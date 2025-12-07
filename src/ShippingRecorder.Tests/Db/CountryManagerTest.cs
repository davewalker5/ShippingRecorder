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
    public class CountryManagerTest
    {
        private const string Code = "GB";
        private const string Name = "United Kingdom";
        private const string SecondCode = "ES";
        private const string SecondName = "Spain";

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _ = await _factory.Countries.AddAsync(Code, Name);
        }

        [TestMethod]
        public async Task CannotAddDuplicateTestAsync()
            => await Assert.ThrowsAsync<CountryExistsException>(() => _factory.Countries.AddAsync(Code, Name));

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Countries.GetAsync(e => e.Code == Code);
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(Code, entity.Code);
            Assert.AreEqual(Name, entity.Name);
        }

        [TestMethod]
        public async Task CannotAddWithNonAlphaCodeTestAsync()
            => await Assert.ThrowsAsync<InvalidCountryCodeException>(() => _factory.Countries.AddAsync("12", Name));

        [TestMethod]
        public async Task CannotAddWithShortCodeTestAsync()
            => await Assert.ThrowsAsync<InvalidCountryCodeException>(() => _factory.Countries.AddAsync(Code[..1], Name));

        [TestMethod]
        public async Task CannotAddWithLongCodeTestAsync()
            => await Assert.ThrowsAsync<InvalidCountryCodeException>(() => _factory.Countries.AddAsync($"{Code}Z", Name));

        [TestMethod]
        public async Task AddExistingIfNotExistTestAsync()
        {
            _ = await _factory.Countries.AddIfNotExistsAsync(Code, Name);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Countries.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddMissingIfNotExistTestAsync()
        {
            var entity = await _factory.Countries.AddIfNotExistsAsync(SecondCode, SecondName);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Countries.Count();
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(SecondCode, entity.Code);
            Assert.AreEqual(SecondName, entity.Name);
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.Countries.GetAsync(e => e.Code == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Countries.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(Code, entities.First().Code);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Countries.ListAsync(e => e.Code == Code, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(Code, entities.First().Code);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Countries.ListAsync(e => e.Code == "Missing", 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Countries.GetAsync(e => e.Code == Code);
            var updated = await _factory.Countries.UpdateAsync(entity.Id, SecondCode, SecondName);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(SecondCode, entity.Code);
            Assert.AreEqual(SecondName, entity.Name);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<CountryNotFoundException>(() => _factory.Countries.UpdateAsync(-1, SecondCode, SecondName));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await _factory.Countries.GetAsync(e => e.Code == Code);
            await _factory.Countries.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Countries.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<CountryNotFoundException>(() => _factory.Countries.DeleteAsync(-1));

    }
}

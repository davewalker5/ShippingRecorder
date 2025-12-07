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
    public class PortManagerTest
    {
        private long _countryId;
        private const string Code = "GBSOU";
        private const string Name = "Southampton";
        private long _secondCountryId;
        private const string SecondCode = "NLRTM";
        private const string SecondName = "Rotterdam";

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _countryId = (await _factory.Countries.AddAsync("GB", "United Kingdom")).Id;
            _secondCountryId = (await _factory.Countries.AddAsync("NL", "The Netherlands")).Id;
            _ = await _factory.Ports.AddAsync(_countryId, Code, Name);
        }

        [TestMethod]
        public async Task CannotAddDuplicateTestAsync()
            => await Assert.ThrowsAsync<PortExistsException>(() => _factory.Ports.AddAsync(_countryId, Code, Name));

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Ports.GetAsync(e => e.Code == Code);
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(_countryId, entity.CountryId);
            Assert.AreEqual(Code, entity.Code);
            Assert.AreEqual(Name, entity.Name);
        }

        [TestMethod]
        public async Task AddExistingIfNotExistTestAsync()
        {
            _ = await _factory.Ports.AddIfNotExistsAsync(_countryId, Code, Name);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Ports.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddMissingIfNotExistTestAsync()
        {
            var entity = await _factory.Ports.AddIfNotExistsAsync(_secondCountryId, SecondCode, SecondName);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Ports.Count();
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(_secondCountryId, entity.CountryId);
            Assert.AreEqual(SecondCode, entity.Code);
            Assert.AreEqual(SecondName, entity.Name);
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.Ports.GetAsync(e => e.Code == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Ports.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(_countryId, entities.First().CountryId);
            Assert.AreEqual(Code, entities.First().Code);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Ports.ListAsync(e => e.Code == Code, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(_countryId, entities.First().CountryId);
            Assert.AreEqual(Code, entities.First().Code);
            Assert.AreEqual(Name, entities.First().Name);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Ports.ListAsync(e => e.Code == "Missing", 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Ports.GetAsync(e => e.Code == Code);
            var updated = await _factory.Ports.UpdateAsync(entity.Id, _secondCountryId, SecondCode, SecondName);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(_secondCountryId, entity.CountryId);
            Assert.AreEqual(SecondCode, entity.Code);
            Assert.AreEqual(SecondName, entity.Name);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<PortNotFoundException>(() => _factory.Ports.UpdateAsync(-1, _secondCountryId, SecondCode, SecondName));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await _factory.Ports.GetAsync(e => e.Code == Code);
            await _factory.Ports.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Ports.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<PortNotFoundException>(() => _factory.Ports.DeleteAsync(-1));

    }
}

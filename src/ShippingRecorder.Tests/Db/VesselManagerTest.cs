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
    public class VesselManagerTest
    {
        private const string VesselIdentifier = "8420878";
        private const int Built = 2005;
        private const int Length = 285;
        private const int Beam = 32;
        private const decimal Draught = 8M;
        private const string SecondVesselIdentifier = "9226906";
        private const int SecondBuilt = 2010;
        private const int SecondLength = 300;
        private const int SecondBeam = 40;
        private const decimal SecondDraught = 10M;

        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _ = await _factory.Vessels.AddAsync(VesselIdentifier, true, Built, Draught, Length, Beam);
        }

        [TestMethod]
        public async Task CannotAddDuplicateTestAsync()
            => await Assert.ThrowsAsync<VesselExistsException>(() => _factory.Vessels.AddAsync(VesselIdentifier, true, Built, Draught, Length, Beam));

        [TestMethod]
        public async Task CannotAddWithNonNumericIdentifierTestAsync()
            => await Assert.ThrowsAsync<InvalidVesselIdentifierException>(() => _factory.Vessels.AddAsync("Invalid", true, null, null, null, null));

        [TestMethod]
        public async Task CannotAddWithShortIdentiiferTestAsync()
            => await Assert.ThrowsAsync<InvalidVesselIdentifierException>(() => _factory.Vessels.AddAsync(VesselIdentifier[..6], true, null, null, null, null));

        [TestMethod]
        public async Task CannotAddWithLongIdentifierTestAsync()
            => await Assert.ThrowsAsync<InvalidVesselIdentifierException>(() => _factory.Vessels.AddAsync($"{VesselIdentifier}0", true, null, null, null, null));

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var entity = await _factory.Vessels.GetAsync(e => e.Identifier == VesselIdentifier);
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(VesselIdentifier, entity.Identifier);
            Assert.AreEqual(Built, entity.Built);
            Assert.AreEqual(Draught, entity.Draught);
            Assert.AreEqual(Length, entity.Length);
            Assert.AreEqual(Beam, entity.Beam);
        }

        [TestMethod]
        public async Task AddExistingIfNotExistTestAsync()
        {
            _ = await _factory.Vessels.AddIfNotExistsAsync(VesselIdentifier, true, Built, Draught, Length, Beam);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Vessels.Count();
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public async Task AddMissingIfNotExistTestAsync()
        {
            var entity = await _factory.Vessels.AddIfNotExistsAsync(SecondVesselIdentifier, true, SecondBuilt, SecondDraught, SecondLength, SecondBeam);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Vessels.Count();
            Assert.IsNotNull(entity);
            Assert.IsGreaterThan(0, entity.Id);
            Assert.AreEqual(SecondVesselIdentifier, entity.Identifier);
            Assert.AreEqual(SecondBuilt, entity.Built);
            Assert.AreEqual(SecondDraught, entity.Draught);
            Assert.AreEqual(SecondLength, entity.Length);
            Assert.AreEqual(SecondBeam, entity.Beam);
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task GetMissingTestAsync()
        {
            var entity = await _factory.Vessels.GetAsync(e => e.Identifier == "Missing");
            Assert.IsNull(entity);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var entities = await _factory.Vessels.ListAsync(e => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(VesselIdentifier, entities.First().Identifier);
            Assert.AreEqual(Built, entities.First().Built);
            Assert.AreEqual(Draught, entities.First().Draught);
            Assert.AreEqual(Length, entities.First().Length);
            Assert.AreEqual(Beam, entities.First().Beam);
        }

        [TestMethod]
        public async Task FilteredListTestAsync()
        {
            var entities = await _factory.Vessels.ListAsync(e => e.Identifier == VesselIdentifier, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, entities);
            Assert.AreEqual(VesselIdentifier, entities.First().Identifier);
            Assert.AreEqual(Built, entities.First().Built);
            Assert.AreEqual(Draught, entities.First().Draught);
            Assert.AreEqual(Length, entities.First().Length);
            Assert.AreEqual(Beam, entities.First().Beam);
        }

        [TestMethod]
        public async Task ListMissingTestAsync()
        {
            var entities = await _factory.Vessels.ListAsync(e => e.Identifier == "Missing", 1, int.MaxValue).ToListAsync();
            Assert.IsEmpty(entities);
        }

        [TestMethod]
        public async Task UpdateTestAsync()
        {
            var entity = await _factory.Vessels.GetAsync(e => e.Identifier == VesselIdentifier);
            var updated = await _factory.Vessels.UpdateAsync(entity.Id, SecondVesselIdentifier, true, SecondBuilt, SecondDraught, SecondLength, SecondBeam);
            Assert.IsNotNull(updated);
            Assert.AreEqual(entity.Id, updated.Id);
            Assert.AreEqual(SecondVesselIdentifier, entity.Identifier);
            Assert.AreEqual(SecondBuilt, entity.Built);
            Assert.AreEqual(SecondDraught, entity.Draught);
            Assert.AreEqual(SecondLength, entity.Length);
            Assert.AreEqual(SecondBeam, entity.Beam);
        }

        [TestMethod]
        public async Task UpdateMissingTestAsync()
            => await Assert.ThrowsAsync<VesselNotFoundException>(() => _factory.Vessels.UpdateAsync(-1, SecondVesselIdentifier, true, SecondBuilt, SecondDraught, SecondLength, SecondBeam));

        [TestMethod]
        public async Task DeleteTestAsync()
        {
            var entity = await _factory.Vessels.GetAsync(e => e.Identifier == VesselIdentifier);
            await _factory.Vessels.DeleteAsync(entity.Id);
            var count = _factory.GetContext<ShippingRecorderDbContext>().Vessels.Count();
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task DeleteMissingTestAsync()
            => await Assert.ThrowsAsync<VesselNotFoundException>(() => _factory.Vessels.DeleteAsync(-1));
    }
}

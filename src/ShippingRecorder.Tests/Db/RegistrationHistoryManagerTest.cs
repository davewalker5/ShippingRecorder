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
    public class RegistrationHistoryManagerTest
    {
        private const string VesselIdentifier = "9226906";
        private const string VesselType = "Passenger (Cruise) Ship";
        private const string UpdatedVesselType = "Vista-Class Cruise Ship";
        private const string CountryCode = "BM";
        private const string UpdatedCountryCode = "GB";
        private const string Operator = "Carnival UK";
        private const string UpdatedOperator = "P&O Cruises UK";
        private const string Name = "Queen Victoria";
        private const string UpdatedName = "Arcadia";
        private const string Callsign = "ZCDN1";
        private const string UpdatedCallsign = "ZCDN2";
        private const string MMSI = "310459000";
        private const string UpdatedMMSI = "000954013";
        private const int Tonnage = 84342;
        private const int UpdatedTonnage = 85000;
        private const int Passengers = 2094;
        private const int UpdatedPassengers = 2458;
        private const int Crew = 866;
        private const int UpdatedCrew = 900;
        private const int Decks = 11;
        private const int UpdatedDecks = 12;
        private const int Cabins = 1050;
        private const int UpdatedCabins = 1100;
        private long _vesselId;
        private long _vesselTypeId;
        private long _updatedVesselTypeId;
        private long _countryId;
        private long _updatedCountryId;
        private long _operatorId;
        private long _updatedOperatorId;
        private ShippingRecorderFactory _factory;

        [TestInitialize]
        public async Task TestInitializeAsync()
        {
            var context = ShippingRecorderDbContextFactory.CreateInMemoryDbContext();
            _factory = new ShippingRecorderFactory(context, new MockFileLogger());
            _vesselId = (await _factory.Vessels.AddAsync(VesselIdentifier, true, null, null, null, null)).Id;

            _vesselTypeId = (await _factory.VesselTypes.AddAsync(VesselType)).Id;
            _updatedVesselTypeId = (await _factory.Operators.AddAsync(UpdatedVesselType)).Id;

            _countryId = (await _factory.Countries.AddAsync(CountryCode, "Bermuda")).Id;
            _updatedCountryId = (await _factory.Countries.AddAsync(UpdatedCountryCode, "United Kingdom")).Id;

            _operatorId = (await _factory.Operators.AddAsync(Operator)).Id;
            _updatedOperatorId = (await _factory.Operators.AddAsync(UpdatedOperator)).Id;

            _ = await _factory.RegistrationHistory.AddAsync(_vesselId, _vesselTypeId, _countryId, _operatorId, Name, Callsign, MMSI, Tonnage, Passengers, Crew, Decks, Cabins);
        }

        [TestMethod]
        public async Task AddAndGetTestAsync()
        {
            var registration = await _factory.RegistrationHistory.GetAsync(x => true);
            Assert.IsNotNull(registration);
            Assert.IsGreaterThan(0, registration.Id);
            Assert.AreEqual(_vesselId, registration.VesselId);
            Assert.AreEqual(_vesselTypeId, registration.VesselTypeId);
            Assert.AreEqual(_countryId, registration.FlagId);
            Assert.AreEqual(_operatorId, registration.OperatorId);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreEqual(Callsign, registration.Callsign);
            Assert.AreEqual(MMSI, registration.MMSI);
            Assert.AreEqual(Tonnage, registration.Tonnage);
            Assert.AreEqual(Passengers, registration.Passengers);
            Assert.AreEqual(Crew, registration.Crew);
            Assert.AreEqual(Decks, registration.Decks);
            Assert.AreEqual(Cabins, registration.Cabins);
            Assert.IsTrue(registration.IsActive);
        }

        [TestMethod]
        public async Task CannotAddWithNonNumericMMSITestAsync()
            => await Assert.ThrowsAsync<InvalidMMSIException>(() => _factory.RegistrationHistory.AddAsync(_vesselId, _vesselTypeId, _countryId, _operatorId, Name, Callsign, "Invalid01", Tonnage, Passengers, Crew, Decks, Cabins));

        [TestMethod]
        public async Task CannotAddWithShortMMSITestAsync()
            => await Assert.ThrowsAsync<InvalidMMSIException>(() => _factory.RegistrationHistory.AddAsync(_vesselId, _vesselTypeId, _countryId, _operatorId, Name, Callsign, $"{MMSI[..8]}", Tonnage, Passengers, Crew, Decks, Cabins));


        [TestMethod]
        public async Task CannotAddWithLongMMSITestAsync()
            => await Assert.ThrowsAsync<InvalidMMSIException>(() => _factory.RegistrationHistory.AddAsync(_vesselId, _vesselTypeId, _countryId, _operatorId, Name, Callsign, $"{MMSI}0", Tonnage, Passengers, Crew, Decks, Cabins));

        [TestMethod]
        public async Task OnlyOneActiveHistoryPerVesselTest()
        {
            _ = await _factory.RegistrationHistory.AddAsync(_vesselId, _updatedVesselTypeId, _updatedCountryId, _updatedOperatorId, UpdatedName, UpdatedCallsign, UpdatedMMSI, UpdatedTonnage, UpdatedPassengers, UpdatedCrew, UpdatedDecks, UpdatedCabins);
            var registrations = await _factory.RegistrationHistory.ListAsync(x => true, 1, int.MaxValue).ToListAsync();

            var initial = await _factory.RegistrationHistory.GetAsync(x => x.MMSI == MMSI);
            Assert.AreEqual(_vesselId, initial.VesselId);
            Assert.AreEqual(_vesselTypeId, initial.VesselTypeId);
            Assert.AreEqual(_countryId, initial.FlagId);
            Assert.AreEqual(_operatorId, initial.OperatorId);
            Assert.AreEqual(Name, initial.Name);
            Assert.AreEqual(Callsign, initial.Callsign);
            Assert.AreEqual(Tonnage, initial.Tonnage);
            Assert.AreEqual(Passengers, initial.Passengers);
            Assert.AreEqual(Crew, initial.Crew);
            Assert.AreEqual(Decks, initial.Decks);
            Assert.AreEqual(Cabins, initial.Cabins);
            Assert.IsFalse(initial.IsActive);

            var updated = await _factory.RegistrationHistory.GetAsync(x => x.MMSI == UpdatedMMSI);
            Assert.AreNotEqual(updated.Id, initial.Id);
            Assert.AreEqual(_vesselId, updated.VesselId);
            Assert.AreEqual(_updatedVesselTypeId, updated.VesselTypeId);
            Assert.AreEqual(_updatedCountryId, updated.FlagId);
            Assert.AreEqual(_updatedOperatorId, updated.OperatorId);
            Assert.AreEqual(UpdatedName, updated.Name);
            Assert.AreEqual(UpdatedCallsign, updated.Callsign);
            Assert.AreEqual(UpdatedTonnage, updated.Tonnage);
            Assert.AreEqual(UpdatedPassengers, updated.Passengers);
            Assert.AreEqual(UpdatedCrew, updated.Crew);
            Assert.AreEqual(UpdatedDecks, updated.Decks);
            Assert.AreEqual(UpdatedCabins, updated.Cabins);
            Assert.IsTrue(updated.IsActive);
        }

        [TestMethod]
        public async Task ListAllTestAsync()
        {
            var registrations = await _factory.RegistrationHistory.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            Assert.HasCount(1, registrations);
            Assert.IsGreaterThan(0, registrations.First().Id);
            Assert.AreEqual(_vesselId, registrations.First().VesselId);
            Assert.AreEqual(_vesselTypeId, registrations.First().VesselTypeId);
            Assert.AreEqual(_countryId, registrations.First().FlagId);
            Assert.AreEqual(_operatorId, registrations.First().OperatorId);
            Assert.AreEqual(Name, registrations.First().Name);
            Assert.AreEqual(Callsign, registrations.First().Callsign);
            Assert.AreEqual(MMSI, registrations.First().MMSI);
            Assert.AreEqual(Tonnage, registrations.First().Tonnage);
            Assert.AreEqual(Passengers, registrations.First().Passengers);
            Assert.AreEqual(Crew, registrations.First().Crew);
            Assert.AreEqual(Decks, registrations.First().Decks);
            Assert.AreEqual(Cabins, registrations.First().Cabins);
            Assert.IsTrue(registrations.First().IsActive);
        }

        [TestMethod]
        public async Task DeactivateTestAsync()
        {
            var registration = await _factory.RegistrationHistory.GetAsync(x => true);
            await _factory.RegistrationHistory.Deactivate(registration.Id);
            var deactivated = await _factory.RegistrationHistory.GetAsync(x => true);
            Assert.IsFalse(deactivated.IsActive);
        }

        [TestMethod]
        public async Task DeactivateMissingTestAsync()
            => await Assert.ThrowsAsync<RegistrationHistoryNotFoundException>(() => _factory.RegistrationHistory.Deactivate(-1));
    }
}
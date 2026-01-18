using System;
using System.Text;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Tests.Mocks
{
    public static class DataGenerator
    {
        private const int MinimumStringLength = 10;
        private const int MaximumStringLength = 50;
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private static readonly string CharacterSet = $"{Letters}1234567890 !@£$%^&*()_-";

        private static Random _generator = new();

        /// <summary>
        /// Return a random integer in the specified range
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static int RandomInt(int minimum, int maximum)
            => _generator.Next(minimum, maximum);

        /// <summary>
        /// Return a random entity Id
        /// </summary>
        /// <returns></returns>
        public static long RandomId()
            => RandomInt(1, int.MaxValue);

        /// <summary>
        /// Return a random decimal in the range 0.0 < d < 1.0
        /// </summary>
        /// <returns></returns>
        private static decimal NextDecimal()
            => (decimal)_generator.NextSingle();

        /// <summary>
        /// Return a random integer in the specified range
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static decimal RandomDecimal(decimal minimum, decimal maximum)
            => minimum + NextDecimal() * (maximum - minimum);

        /// <summary>
        /// Generate a random alphanumeric word of the specified length
        /// </summary>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <param name="characters"><param>
        /// <returns></returns>
        public static string RandomWord(int minimumLength, int maximumLength, string characters)
        {
            // Generate a random length for the word, within the specified limits
            var length = RandomInt(minimumLength, maximumLength);

            // Iterate over the number of characters
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                // Select a random offset within the character set and append that character
                var offset = (int)_generator.NextInt64(0, CharacterSet.Length);
                builder.Append(CharacterSet[offset]);
            }

            return builder.ToString().Trim();
        }

        /// <summary>
        /// Generate a random alphanumeric word of the specified length
        /// </summary>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static string RandomWord(int minimumLength, int maximumLength)
            => RandomWord(minimumLength, maximumLength, CharacterSet);

        /// <summary>
        /// Generate a random word of the specified length containing only letters
        /// </summary>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static string RandomAlphaWord(int minimumLength, int maximumLength)
            => RandomWord(minimumLength, maximumLength, Letters);

        /// <summary>
        /// Generate a random alphanumeric word
        /// </summary>
        /// <returns></returns>
        public static string RandomWord()
            => RandomWord(MinimumStringLength, MaximumStringLength);

        /// <summary>
        /// Return a random location
        /// </summary>
        /// <returns></returns>
        public static Location CreateLocation()
            => new() { Id = RandomId(), Name = RandomWord().TitleCase() };

        /// <summary>
        /// Return a random operator
        /// </summary>
        /// <returns></returns>
        public static Operator CreateOperator()
            => new() { Id = RandomId(), Name = RandomWord().TitleCase() };

        /// <summary>
        /// Return a random vessel type
        /// </summary>
        /// <returns></returns>
        public static VesselType CreateVesselType()
            => new() { Id = RandomId(), Name = RandomWord().TitleCase() };

        /// <summary>
        /// Return a random country
        /// </summary>
        /// <returns></returns>
        public static Country CreateCountry()
            => new() { Id = RandomId(), Code = RandomAlphaWord(2, 2).CleanCode(), Name = RandomWord().TitleCase() };

        /// <summary>
        /// Return a random port
        /// </summary>
        /// <returns></returns>
        public static Port CreatePort()
        {
            var country = CreateCountry();
            return new()
            {
                Id = RandomId(), 
                CountryId = country.Id,
                Country = country,
                Code = $"{country.Code}{RandomWord(3, 3)}".CleanCode(),
                Name = RandomWord().TitleCase()
            };
        }

        /// <summary>
        /// Return a random voyage
        /// </summary>
        /// <returns></returns>
        public static Voyage CreateVoyage()
        {
            var op = CreateOperator();
            var vessel = CreateVessel();
            var evt = CreateVoyageEvent(RandomId());
            return new()
            {
                Id = evt.VoyageId,
                OperatorId = op.Id,
                Operator = op,
                VesselId = vessel.Id,
                Vessel = vessel,
                Number = RandomWord().CleanCode(),
                Events = [evt]
            };
        }

        /// <summary>
        /// Return a random voyage event for the voyage with the specified Id
        /// </summary>
        /// <param name="voyageId"></param>
        /// <returns></returns>
        public static VoyageEvent CreateVoyageEvent(long voyageId)
        {
            var port = CreatePort();
            return new()
            {
                Id = RandomId(),
                VoyageId = voyageId,
                EventType = RandomInt(0, 100) < 50 ? VoyageEventType.Depart : VoyageEventType.Arrive,
                PortId = port.Id,
                Port = port,
                Date = DateTime.Now
            };
        }

        /// <summary>
        /// Return a random voyage event
        /// </summary>
        /// <param name="voyageId"></param>
        /// <returns></returns>
        public static VoyageEvent CreateVoyageEvent()
            => CreateVoyageEvent(RandomId());

        /// <summary>
        /// Create a random vessel registration history
        /// </summary>
        /// <returns></returns>
        public static RegistrationHistory CreateRegistrationHistory()
        {
            var vesselType = CreateVesselType();
            var flag = CreateCountry();
            var op = CreateOperator();
            return new()
            {
                Id = RandomId(),
                VesselId = RandomId(),
                VesselType = vesselType,
                VesselTypeId = vesselType.Id,
                Flag = flag,
                FlagId = flag.Id,
                Operator = op,
                OperatorId = op.Id,
                Date = DateTime.Today,
                Name = RandomWord(),
                Callsign = RandomWord().CleanCode(),
                MMSI = RandomWord(9, 9).CleanCode(),
                Tonnage = RandomInt(40000, 80000),
                Crew = RandomInt(500, 1000),
                IsActive = true
            };
        }

        /// <summary>
        /// Create a random vessel
        /// </summary>
        /// <returns></returns>
        public static Vessel CreateVessel()
        {
            var vessel = new Vessel
            {
                Id = RandomId(),
                Identifier = RandomInt(0, 9999999).ToString("0000000"),
                IsIMO = true,
                Built = 1950 + RandomInt(0, DateTime.Today.Year - 1950),
                Draught = RandomDecimal(2M, 10M),
                Length = RandomInt(5, 300),
                Beam = RandomInt(2, 35)
            };

            var registration = CreateRegistrationHistory();
            registration.VesselId = vessel.Id;
            vessel.RegistrationHistory = [registration];

            return vessel;
        }

        /// <summary>
        /// Create a random sighting
        /// </summary>
        /// <returns></returns>
        public static Sighting CreateSighting()
        {
            var location = CreateLocation();
            var vessel = CreateVessel();
            return new()
            {
                Id = RandomId(),
                Location = location,
                LocationId = location.Id,
                Vessel = vessel,
                VesselId = vessel.Id,
                Date = DateTime.Today,
                IsMyVoyage = RandomInt(0, 100) > 50
            };
        }
    }
}
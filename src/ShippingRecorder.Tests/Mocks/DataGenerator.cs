using System;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Tests.Mocks
{
    public static class DataGenerator
    {
        private const int MinimumStringLength = 10;
        private const int MaximumStringLength = 50;
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 !@£$%^&*()_-";

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
        /// <returns></returns>
        public static string RandomWord(int minimumLength, int maximumLength)
        {
            // Generate a random length for the word, within the specified limits
            var length = RandomInt(minimumLength, maximumLength);

            // Iterate over the number of characters
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                // Select a random offset within the character set and append that character
                var offset = (int)_generator.NextInt64(0, Letters.Length);
                builder.Append(Letters[offset]);
            }

            return builder.ToString();
        }

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
            => new() { Id = RandomId(), Name = RandomWord() };

        /// <summary>
        /// Return a random operator
        /// </summary>
        /// <returns></returns>
        public static Operator CreateOperator()
            => new() { Id = RandomId(), Name = RandomWord() };

        /// <summary>
        /// Return a random vessel type
        /// </summary>
        /// <returns></returns>
        public static VesselType CreateVesselType()
            => new() { Id = RandomId(), Name = RandomWord() };

        /// <summary>
        /// Return a random country
        /// </summary>
        /// <returns></returns>
        public static Country CreateCountry()
            => new() { Id = RandomId(), Code = RandomWord(2, 2), Name = RandomWord() };

        /// <summary>
        /// Return a random port
        /// </summary>
        /// <returns></returns>
        public static Port CreatePort()
            => new() { Id = RandomId(), CountryId = RandomId(), Code = RandomWord(2, 2), Name = RandomWord() };

        /// <summary>
        /// Return a random voyage
        /// </summary>
        /// <returns></returns>
        public static Voyage CreateVoyage()
            => new() { Id = RandomId(), OperatorId = RandomId(), Number = RandomWord() };

        /// <summary>
        /// Return a random voyage
        /// </summary>
        /// <returns></returns>
        public static VoyageEvent CreateVoyageEvent()
            => new()
            {
                Id = RandomId(),
                VoyageId = RandomId(),
                EventType = RandomInt(0, 100) < 50 ? VoyageEventType.Depart : VoyageEventType.Arrive,
                PortId = RandomId(),
                Date = DateTime.Now
            };

        /// <summary>
        /// Create a random vessel registration history
        /// </summary>
        /// <returns></returns>
        public static RegistrationHistory CreateRegistrationHistory()
            => new()
            {
                Id = RandomId(),
                VesselId = RandomId(),
                VesselTypeId = RandomId(),
                FlagId = RandomId(),
                OperatorId = RandomId(),
                Date = DateTime.Now,
                Name = RandomWord(),
                Callsign = RandomWord(),
                MMSI = RandomWord(9, 9),
                Tonnage = RandomInt(40000, 80000),
                Crew = RandomInt(500, 1000),
            };

        /// <summary>
        /// Create a random vessel
        /// </summary>
        /// <returns></returns>
        public static Vessel CreateVessel()
            => new()
            {
                Id = RandomId(),
                IMO = RandomInt(0, 9999999).ToString("0000000"),
                Built = 1950 + RandomInt(0, DateTime.Today.Year - 1950),
                Draught = RandomDecimal(0.5M, 10M),
                Length = RandomInt(30, 300),
                Beam = RandomInt(1, 35)
            };

        /// <summary>
        /// Create a random sighting
        /// </summary>
        /// <returns></returns>
        public static Sighting CreateSighting()
            => new()
            {
                Id = RandomId(),
                LocationId = RandomId(),
                VoyageId = RandomId(),
                VesselId = RandomId(),
                Date = DateTime.Now,
                IsMyVoyage = RandomInt(0, 100) > 50
            };
    }
}
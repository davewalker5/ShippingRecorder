using System;
using System.Text;
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
    }
}
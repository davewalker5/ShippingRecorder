using System;

namespace ShippingRecorder.BusinessLogic.Extensions
{
    public static partial class DecimalExtensions
    {
        /// <summary>
        /// Check the decimal value is in range
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        public static bool ValidateDecimal(this decimal? input, decimal minimum, decimal maximum, bool allowNull)
            => ((input == null) && allowNull) || ((input >= minimum) && (input <= maximum));

        /// <summary>
        /// Check a decimal value is in range and throw an exception if not
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static void ValidateDecimalAndThrow<T>(this decimal? input, decimal minimum, decimal maximum, bool allowNull) where T : Exception
        {
            if (!ValidateDecimal(input, minimum, maximum, allowNull))
            {
                var orNull = allowNull ? " or null" : "";
                var message = $"'{input}' is not a decimal between {minimum} and {maximum}{orNull}";
                var exception = (T)Activator.CreateInstance(typeof(T), message);
                throw exception;
            }
        }
    }
}
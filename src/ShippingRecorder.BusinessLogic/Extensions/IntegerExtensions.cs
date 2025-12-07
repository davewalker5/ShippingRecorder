using System;

namespace ShippingRecorder.BusinessLogic.Extensions
{
    public static partial class IntegerExtensions
    {
        /// <summary>
        /// Check an integer value is in range
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        public static bool ValidateInteger(this int? input, int minimum, int maximum, bool allowNull)
            => ((input == null) && allowNull) || ((input >= minimum) && (input <= maximum));

        /// <summary>
        /// Check an integer value is in range and throw an exception if not
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static void ValidateIntegerAndThrow<T>(this int? input, int minimum, int maximum, bool allowNull) where T : Exception
        {
            if (!ValidateInteger(input, minimum, maximum, allowNull))
            {
                var orNull = allowNull ? " or null" : "";
                var message = $"'{input}' is not an integer between {minimum} and {maximum}{orNull}";
                var exception = (T)Activator.CreateInstance(typeof(T), message);
                throw exception;
            }
        }
    }
}
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShippingRecorder.BusinessLogic.Extensions
{
    public static partial class StringExtensions
    {
        [GeneratedRegex("^[0-9]+$")]
        private static partial Regex NumericRegex();

        [GeneratedRegex("^[A-Za-z]+$")]
        private static partial Regex AlphaRegex();

        /// <summary>
        /// Clean a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Clean(this string input)
        {
            input = input.Trim().Replace("\t", "").Replace("\n", "").Replace("\r", "");
            while (input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }
            return input;
        }

        /// <summary>
        /// Clean a string representing a code e.g. country, port
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string CleanCode(this string code)
            => code.Clean().Replace(" ", "").ToUpper();

        /// <summary>
        /// Clean a string and convert to title case
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TitleCase(this string input)
            => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input.Clean().ToLower());

        /// <summary>
        /// Check a string is a valid numeric string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static bool ValidateNumeric(this string input, int minimumLength, int maximumLength)
            => (input.Length >= minimumLength) && (input.Length <= maximumLength) && NumericRegex().IsMatch(input);

        /// <summary>
        /// Check a string is a valid numeric string and throw an exception if not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        public static void ValidateNumericAndThrow<T>(this string input, int minimumLength, int maximumLength) where T: Exception
        {
            if (!ValidateNumeric(input, minimumLength, maximumLength))
            {
                var message = $"'{input}' is not a numeric string between {minimumLength} and {maximumLength} characters long";
                var exception = (T)Activator.CreateInstance(typeof(T), message);
                throw exception;
            }
        }

        /// <summary>
        /// Check a string is a valid alpha-character string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static bool ValidateAlpha(this string input, int minimumLength, int maximumLength)
            => (input.Length >= minimumLength) && (input.Length <= maximumLength) && AlphaRegex().IsMatch(input);

        /// <summary>
        /// Check a string is a valid numeric string and throw an exception if not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="minimumLength"></param>
        /// <param name="maximumLength"></param>
        public static void ValidateAlphaAndThrow<T>(this string input, int minimumLength, int maximumLength) where T: Exception
        {
            if (!ValidateAlpha(input, minimumLength, maximumLength))
            {
                var message = $"'{input}' is not an alpha-character string between {minimumLength} and {maximumLength} characters long";
                var exception = (T)Activator.CreateInstance(typeof(T), message);
                throw exception;
            }
        }
    }
}

using System.Collections.Generic;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Extensions
{
    public static class CountryExtensions
    {
        /// <summary>
        /// Return an exportable country from a country
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static ExportableCountry ToExportable(this Country country)
            => new()
            {
                Code = country.Code,
                Name = country.Name
            };

        /// <summary>
        /// Return a collection of exportable countries from a collection of countries
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableCountry> ToExportable(this IEnumerable<Country> countries)
        {
            var exportable = new List<ExportableCountry>();

            foreach (var country in countries)
            {
                exportable.Add(country.ToExportable());
            }

            return exportable;
        }
    }
}

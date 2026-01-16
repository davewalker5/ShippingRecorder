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
        /// Return a collection of exportable countrys from a collection of countrys
        /// </summary>
        /// <param name="countrys"></param>
        /// <returns></returns>
        public static IEnumerable<ExportableCountry> ToExportable(this IEnumerable<Country> countrys)
        {
            var exportable = new List<ExportableCountry>();

            foreach (var country in countrys)
            {
                exportable.Add(country.ToExportable());
            }

            return exportable;
        }
    }
}

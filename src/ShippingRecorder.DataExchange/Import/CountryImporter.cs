using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class CountryImporter : CsvImporter<ExportableCountry>, ICountryImporter
    {
        private List<Country> _countries;

        public CountryImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
        {
        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _countries = await _factory.Countries.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableCountry Inflate(string record)
            => ExportableCountry.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="country"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableCountry country, int recordCount)
        {
            ValidateField<string>(x => x.ValidateAlpha(2, 2), country.Code,  "Code", recordCount);
            ValidateField<string>(x => CheckCountryDoesNotExist(x), country.Code,  "Code", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableCountry country)
            => await _factory.Countries.AddAsync(country.Code, country.Name);

        /// <summary>
        /// Check a country does not exist
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckCountryDoesNotExist(string code)
            => _countries.FirstOrDefault(x => x.Code == code) == null;
    }
}
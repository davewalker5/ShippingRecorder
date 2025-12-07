using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class PortImporter : CsvImporter<ExportablePort>, IPortImporter
    {
        private List<Country> _countries;

        public PortImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
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
        protected override ExportablePort Inflate(string record)
            => ExportablePort.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="port"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportablePort port, int recordCount)
        {
            // Port codes are the country code (2 characters) followed by the location code
            ValidateField<string>(x => CheckPortDoesNotExist(x), port.Code,  "Code", recordCount);
            ValidateField<string>(x => CheckCountryExists(x[..2]), port.Code,  "Code", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportablePort port)
        {
            var country = _countries.First(x => x.Code == port.Code[..2]);
            await _factory.Ports.AddAsync(country.Id, port.Code, port.Name);
        }

        /// <summary>
        /// Check a vessel exists
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckPortDoesNotExist(string code)
        {
            var port = Task.Run(() => _factory.Ports.GetAsync(x => x.Code == code)).Result;
            return port == null;
        }

        /// <summary>
        /// Check a country exists
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckCountryExists(string code)
            => _countries.FirstOrDefault(x => x.Code == code) != null;
    }
}
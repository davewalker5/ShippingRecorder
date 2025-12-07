using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class LocationImporter : CsvImporter<ExportableLocation>, ILocationImporter
    {
        private List<Location> _locations;

        public LocationImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
        {

        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _locations = await _factory.Locations.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableLocation Inflate(string record)
            => ExportableLocation.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="location"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableLocation location, int recordCount)
        {
            ValidateField<string>(x => CheckLocationDoesNotExist(x), location.Name,  "Name", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableLocation location)
            => await _factory.Locations.AddAsync(location.Name);

        /// <summary>
        /// Check a location does not exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckLocationDoesNotExist(string name)
            => _locations.FirstOrDefault(x => x.Name == name) == null;
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class VesselTypeImporter : CsvImporter<ExportableVesselType>, IVesselTypeImporter
    {
        private List<VesselType> _types;

        public VesselTypeImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
        {
        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _types = await _factory.VesselTypes.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableVesselType Inflate(string record)
            => ExportableVesselType.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="vesselType"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableVesselType vesselType, int recordCount)
        {
            ValidateField<string>(x => CheckVesselTypeDoesNotExist(x), vesselType.Name,  "Name", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="vesselType"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableVesselType vesselType)
            => await _factory.VesselTypes.AddAsync(vesselType.Name);

        /// <summary>
        /// Check a vessel type exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckVesselTypeDoesNotExist(string name)
            => _types.FirstOrDefault(x => x.Name == name) == null;
    }
}
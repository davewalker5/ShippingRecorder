using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class SightingImporter : CsvImporter<ExportableSighting>, ISightingImporter
    {
        private List<Location> _locations;
        private List<Vessel> _vessels;
        private List<Voyage> _voyages;

        public SightingImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
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
            _vessels = await _factory.Vessels.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            _voyages = await _factory.Voyages.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableSighting Inflate(string record)
            => ExportableSighting.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="sighting"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableSighting sighting, int recordCount)
        {
            ValidateField<DateTime>(x => x <= DateTime.Now, sighting.Date, "Date", recordCount);
            ValidateField<string>(x => CheckLocationExists(x), sighting.Location, "Location", recordCount);
            ValidateField<string>(x => CheckVesselExists(x), sighting.IMO, "IMO", recordCount);
            ValidateField<string>(x => CheckVoyageExists(x), sighting.VoyageNumber, "VoyageNumber", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableSighting sighting)
        {
            var location = _locations.First(x => x.Name == sighting.Location);
            var vessel = _vessels.First(x => x.IMO == sighting.IMO);
            long? voyageId = string.IsNullOrEmpty(sighting.VoyageNumber) ? null : _voyages.First(x => x.Number == sighting.VoyageNumber).Id;
            await _factory.Sightings.AddAsync(location.Id, voyageId, vessel.Id, sighting.Date, sighting.IsMyVoyage);
        }

        /// <summary>
        /// Check a location exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckLocationExists(string name)
            => _locations.FirstOrDefault(x => x.Name == name) != null;

        /// <summary>
        /// Check a vessel exists
        /// </summary>
        /// <param name="imo"></param>
        /// <returns></returns>
        private bool CheckVesselExists(string imo)
            => _vessels.FirstOrDefault(x => x.IMO == imo) != null;

        /// <summary>
        /// Check a voyage exists
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private bool CheckVoyageExists(string number)
            => string.IsNullOrEmpty(number) || _voyages.FirstOrDefault(x => x.Number == number) != null;
    }
}
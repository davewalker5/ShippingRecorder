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
    public sealed class VesselImporter : CsvImporter<ExportableVessel>, IVesselImporter
    {
        private List<VesselType> _types;
        private List<Country> _countries;
        private List<Operator> _operators;

        public VesselImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
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
            _countries = await _factory.Countries.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
            _operators = await _factory.Operators.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableVessel Inflate(string record)
            => ExportableVessel.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="vessel"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableVessel vessel, int recordCount)
        {
            ValidateField<string>(x => x.ValidateNumeric(7, 7), vessel.Identifier, "Identifier", recordCount);
            ValidateField<string>(x => x.ValidateNumeric(9, 9), vessel.MMSI, "MMSI", recordCount);
            ValidateField<string>(x => x.ValidateAlpha(2, 2), vessel.Flag, "Flag", recordCount);
            ValidateField<string>(x => CheckVesselDoesNotExist(x), vessel.Identifier, "Identifier", recordCount);
            ValidateField<string>(x => CheckVesselTypeExists(x), vessel.VesselType, "VesselType", recordCount);
            ValidateField<string>(x => CheckCountryExists(x), vessel.Flag, "Flag", recordCount);
            ValidateField<string>(x => CheckOperatorExists(x), vessel.Operator, "Operator", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="vessel"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableVessel vessel)
        {
            var type = _types.First(x => x.Name == vessel.VesselType);
            var country = _countries.First(x => x.Code == vessel.Flag);
            var op = _operators.First(x => x.Name == vessel.Operator);

            // If the vessel exists, update its properties. Otherwise, create a new vessel
            long id;
            var existing = await _factory.Vessels.GetAsync(x => x.Identifier == vessel.Identifier);
            if (existing != null)
            {
                id = existing.Id;
                await _factory.Vessels.UpdateAsync(id, vessel.Identifier, vessel.IsIMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam);
            }
            else
            {
                id = (await _factory.Vessels.AddAsync(vessel.Identifier, vessel.IsIMO, vessel.Built, vessel.Draught, vessel.Length, vessel.Beam)).Id;     
            }

            // Add the registration history
            _ = await _factory.RegistrationHistory.AddAsync(
                                id,
                                type.Id,
                                country.Id,
                                op.Id,
                                vessel.Name,
                                vessel.Callsign,
                                vessel.MMSI,
                                vessel.Tonnage,
                                vessel.Passengers,
                                vessel.Crew,
                                vessel.Decks,
                                vessel.Cabins);
        }

        /// <summary>
        /// Check a vessel exists
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private bool CheckVesselDoesNotExist(string identifier)
        {
            var vessel = Task.Run(() => _factory.Vessels.GetAsync(x => x.Identifier == identifier)).Result;
            return vessel == null;
        }

        /// <summary>
        /// Check a vessel type exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckVesselTypeExists(string name)
            => _types.FirstOrDefault(x => x.Name == name) != null;

        /// <summary>
        /// Check a country exists
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckCountryExists(string code)
            => _countries.FirstOrDefault(x => x.Code == code) != null;

        /// <summary>
        /// Check an operator exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckOperatorExists(string name)
            => _operators.FirstOrDefault(x => x.Name == name) != null;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;

namespace ShippingRecorder.BusinessLogic.Database
{
    internal class VesselManager : DatabaseManagerBase, IVesselManager
    {

        internal VesselManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Vessel> GetAsync(Expression<Func<Vessel, bool>> predicate)
        {
            List<Vessel> countries = await ListAsync(predicate, 1, 1).ToListAsync();
            return countries.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Vessel> ListAsync(Expression<Func<Vessel, bool>> predicate, int pageNumber, int pageSize)
            => Context.Vessels
                    .Where(predicate)
                    .Include(x => x.RegistrationHistory)
                        .ThenInclude(h => h.VesselType)
                    .Include(x => x.RegistrationHistory)
                        .ThenInclude(h => h.Flag)
                    .Include(x => x.RegistrationHistory)
                        .ThenInclude(h => h.Operator)
                    .OrderBy(x => x.IMO)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsAsyncEnumerable();

        /// <summary>
        /// Add a vessel
        /// </summary>
        /// <param name="imo"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Vessel> AddAsync(string imo, int? built, decimal? draught, int? length, int? beam)
        {
            imo = imo.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Adding vessel: IMO = {imo}, Built = {built}, Draught = {draught}, Length = {length}, Beam = {beam}");

            // Validate the parameters
            imo.ValidateNumericAndThrow<InvalidIMOException>(7, 7);
            built.ValidateIntegerAndThrow<InvalidYearBuiltException>(Vessel.EarliestYearBuilt, DateTime.Today.Year, true);
            draught.ValidateDecimalAndThrow<InvalidDraughtException>(Vessel.MinimumDraught, decimal.MaxValue, true);
            length.ValidateIntegerAndThrow<InvalidLengthException>(Vessel.MinimumLength, int.MaxValue, true);
            beam.ValidateIntegerAndThrow<InvalidBeamException>(Vessel.MinimumBeam, int.MaxValue, true);

            // Check the vessel doesn't already exist
            await CheckVesselIsNotADuplicate(imo, 0);

            // Add the vessel and save changes
            var vessel = new Vessel
            {
                IMO = imo,
                Built = built,
                Draught = draught,
                Length = length,
                Beam = beam
            };
            await Context.Vessels.AddAsync(vessel);
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(vessel).Collection(x => x.RegistrationHistory).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added vessel {vessel}");

            return vessel;
        }

        /// <summary>
        /// Add a vessel, if it doesn't already exist
        /// </summary>
        /// <param name="imo"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Vessel> AddIfNotExistsAsync(string imo, int? built, decimal? draught, int? length, int? beam)
        {
            imo = imo.CleanCode();
            var vessel = await GetAsync(x => x.IMO == imo);
            if (vessel == null)
            {
                vessel = await AddAsync(imo, built, draught, length, beam);
            }
            return vessel;
        }

        /// <summary>
        /// Update a vessel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imo"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        /// <exception cref="VesselNotFoundException"></exception>
        public async Task<Vessel> UpdateAsync(long id, string imo, int? built, decimal? draught, int? length, int? beam)
        {
            imo = imo.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Updating vessel: ID = {id}, IMO = {imo}, Built = {built}, Draught = {draught}, Length = {length}, Beam = {beam}");

            // Validate the IMO
            imo.ValidateNumericAndThrow<InvalidIMOException>(7, 7);

            // Retrieve the vessel
            var vessel = await Context.Vessels.FirstOrDefaultAsync(x => x.Id == id);
            if (vessel == null)
            {
                var message = $"Vessel with ID {id} not found";
                throw new VesselNotFoundException(message);
            }

            // Check the vessel doesn't already exist
            await CheckVesselIsNotADuplicate(imo, id);

            // Update the vessel properties and save changes
            vessel.IMO = imo;
            vessel.Built = built;
            vessel.Draught = draught;
            vessel.Length = length;
            vessel.Beam = beam;
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(vessel).Collection(x => x.RegistrationHistory).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated vessel {vessel}");

            return vessel;
        }

        /// <summary>
        /// Delete the vessel with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="vesselNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting vessel: ID = {id}");

            // Check the vessel exists
            var vessel = await Context.Vessels.FirstOrDefaultAsync(x => x.Id == id);
            if (vessel == null)
            {
                var message = $"Vessel with ID {id} not found";
                throw new VesselNotFoundException(message);
            }

            // Remove the vessel
            Context.Remove(vessel);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate vessel
        /// </summary>
        /// <param imo="imo"></param>
        /// <param name="id"></param>
        /// <exception cref="VesselExistsException"></exception>
        private async Task CheckVesselIsNotADuplicate(string imo, long id)
        {
            var vessel = await Context.Vessels.FirstOrDefaultAsync(x => x.IMO == imo);
            if ((vessel != null) && (vessel.Id != id))
            {
                var message = $"Vessel {imo} already exists";
                throw new VesselExistsException(message);
            }
        }
    }
}

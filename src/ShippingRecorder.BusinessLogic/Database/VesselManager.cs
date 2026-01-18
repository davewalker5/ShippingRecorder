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
                    .OrderBy(x => x.Identifier)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsAsyncEnumerable();

        /// <summary>
        /// Add a vessel
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="isIMO"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Vessel> AddAsync(string identifier, bool isIMO, int? built, decimal? draught, int? length, int? beam)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding vessel: Identifier = {identifier}, Built = {built}, Draught = {draught}, Length = {length}, Beam = {beam}");

            // Validate the parameters
            if (isIMO)
            {
                identifier = identifier.CleanCode();
                identifier.ValidateNumericAndThrow<InvalidVesselIdentifierException>(7, 7);
            }
            else
            {
                identifier = identifier.Clean().ToUpper();
            }

            built.ValidateIntegerAndThrow<InvalidYearBuiltException>(Vessel.EarliestYearBuilt, DateTime.Today.Year, true);
            draught.ValidateDecimalAndThrow<InvalidDraughtException>(Vessel.MinimumDraught, decimal.MaxValue, true);
            length.ValidateIntegerAndThrow<InvalidLengthException>(Vessel.MinimumLength, int.MaxValue, true);
            beam.ValidateIntegerAndThrow<InvalidBeamException>(Vessel.MinimumBeam, int.MaxValue, true);

            // Check the vessel doesn't already exist
            await CheckVesselIsNotADuplicate(identifier, 0);

            // Add the vessel and save changes
            var vessel = new Vessel
            {
                Identifier = identifier,
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
        /// <param name="identifier"></param>
        /// <param name="isIMO"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        public async Task<Vessel> AddIfNotExistsAsync(string identifier, bool isIMO, int? built, decimal? draught, int? length, int? beam)
        {
            identifier = isIMO ? identifier.CleanCode() : identifier.Clean().ToUpper();
            var vessel = await GetAsync(x => x.Identifier == identifier);
            if (vessel == null)
            {
                vessel = await AddAsync(identifier, isIMO, built, draught, length, beam);
            }
            return vessel;
        }

        /// <summary>
        /// Update a vessel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identifier"></param>
        /// <param name="isIMO"></param>
        /// <param name="built"></param>
        /// <param name="draught"></param>
        /// <param name="length"></param>
        /// <param name="beam"></param>
        /// <returns></returns>
        /// <exception cref="VesselNotFoundException"></exception>
        public async Task<Vessel> UpdateAsync(long id, string identifier, bool isIMO, int? built, decimal? draught, int? length, int? beam)
        {
            identifier = identifier.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Updating vessel: ID = {id}, Identifier = {identifier}, Built = {built}, Draught = {draught}, Length = {length}, Beam = {beam}");

            // Validate the identifier
            if (isIMO)
            {
                identifier = identifier.CleanCode();
                identifier.ValidateNumericAndThrow<InvalidVesselIdentifierException>(7, 7);
            }
            else
            {
                identifier = identifier.Clean().ToUpper();
            }

            // Retrieve the vessel
            var vessel = await Context.Vessels.FirstOrDefaultAsync(x => x.Id == id);
            if (vessel == null)
            {
                var message = $"Vessel with ID {id} not found";
                throw new VesselNotFoundException(message);
            }

            // Check the vessel doesn't already exist
            await CheckVesselIsNotADuplicate(identifier, id);

            // Update the vessel properties and save changes
            vessel.Identifier = identifier;
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
        /// <param name="identifier"></param>
        /// <param name="id"></param>
        /// <exception cref="VesselExistsException"></exception>
        private async Task CheckVesselIsNotADuplicate(string identifier, long id)
        {
            var vessel = await Context.Vessels.FirstOrDefaultAsync(x => x.Identifier == identifier);
            if ((vessel != null) && (vessel.Id != id))
            {
                var message = $"Vessel {identifier} already exists";
                throw new VesselExistsException(message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Exceptions;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;

namespace ShippingRecorder.BusinessLogic.Database
{
    internal class SightingManager : DatabaseManagerBase, ISightingManager
    {
        internal SightingManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate)
        {
            List<Sighting> sightings = await ListAsync(predicate, 1, 1).ToListAsync();
            return sightings.FirstOrDefault();
        }

        /// <summary>
        /// Get the most recent sighting matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Sighting> GetMostRecentAsync(Expression<Func<Sighting, bool>> predicate)
            => await ListAsync(predicate, 1, int.MaxValue).OrderByDescending(x => x.Date).FirstOrDefaultAsync();

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize)
            => Context.Sightings
                        .Where(predicate)
                        .Include(x => x.Location)
                        .Include(x => x.Vessel)
                            .ThenInclude(v => v.RegistrationHistory)
                        .Include(x => x.Voyage)
                            .ThenInclude(v => v.Events
                                .OrderBy(e => e.Date)
                                .ThenBy(e => e.EventType))
                                .ThenInclude(e => e.Port)
                        .OrderBy(x => x.Date)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .AsAsyncEnumerable();

        /// <summary>
        /// Add a sighting, if it doesn't already exist
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="voyageId"></param>
        /// <param name="vesselId"></param>
        /// <param name="date"></param>
        /// <param name="isMyVoyage"></param>
        /// <returns></returns>
        public async Task<Sighting> AddAsync(long locationId, long? voyageId, long vesselId, DateTime date, bool isMyVoyage)
        {
            _factory.Logger.LogMessage(Severity.Debug,
                $"Adding sighting: " +
                $"Location ID = {locationId}, " +
                $"Voyage ID = {voyageId}, " +
                $"Vessel ID = {vesselId}, " +
                $"Date = {date}, " +
                $"My Voyage = {isMyVoyage}");

            // Add the sighting and save changes
            var sighting = new Sighting
            {
                LocationId = locationId,
                VoyageId = voyageId,
                VesselId = vesselId,
                Date = date,
                IsMyVoyage = isMyVoyage
            };

            await Context.Sightings.AddAsync(sighting);
            await Context.SaveChangesAsync();

            // Reload to load related entities
            sighting = await GetAsync(x => x.Id == sighting.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"Added sighting {sighting}");

            return sighting;
        }

        /// <summary>
        /// Update a sighting
        /// </summary>
        /// <param name="id"></param>
        /// <param name="locationId"></param>
        /// <param name="voyageId"></param>
        /// <param name="vesselId"></param>
        /// <param name="date"></param>
        /// <param name="isMyVoyage"></param>
        /// <returns></returns>
        /// <exception cref="SightingNotFoundException"></exception>
        public async Task<Sighting> UpdateAsync(long id, long locationId, long? voyageId, long vesselId, DateTime date, bool isMyVoyage)
        {
            _factory.Logger.LogMessage(Severity.Debug, 
                $"Updating sighting {id} : " +
                $"Location ID = {locationId}, " +
                $"Voyage ID = {voyageId}, " +
                $"Vessel ID = {vesselId}, " +
                $"Date = {date}, " +
                $"My Voyage = {isMyVoyage}");

            // Retrieve the sighting
            var sighting = await Context.Sightings.FirstOrDefaultAsync(x => x.Id == id);
            if (sighting == null)
            {
                var message = $"Sighting with ID {id} not found";
                throw new SightingNotFoundException(message);
            }

            // Update the sighting properties and save changes
            sighting.LocationId = locationId;
            sighting.VoyageId = voyageId;
            sighting.VesselId = vesselId;
            sighting.Date = date;
            sighting.IsMyVoyage = isMyVoyage;
            await Context.SaveChangesAsync();

            // Reload to load related entities
            sighting = await GetAsync(x => x.Id == sighting.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"Updated sighting {sighting}");

            return sighting;
        }

        /// <summary>
        /// Delete the sighting with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting sighting: ID = {id}");

            // Check the sighting exists
            var sighting = await Context.Sightings.FirstOrDefaultAsync(x => x.Id == id);
            if (sighting == null)
            {
                var message = $"Sighting with ID {id} not found";
                throw new SightingNotFoundException(message);
            }

            // Remove the port
            Context.Remove(sighting);
            await Context.SaveChangesAsync();
        }
    }
}
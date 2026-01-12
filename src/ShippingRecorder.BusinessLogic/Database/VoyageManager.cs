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
    internal class VoyageManager : DatabaseManagerBase, IVoyageManager
    {
        internal VoyageManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Voyage> GetAsync(Expression<Func<Voyage, bool>> predicate)
        {
            List<Voyage> voyages = await ListAsync(predicate, 1, 1).ToListAsync();
            return voyages.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Voyage> ListAsync(Expression<Func<Voyage, bool>> predicate, int pageNumber, int pageSize)
            => Context.Voyages
                            .Where(predicate)
                            .Include(x => x.Events
                                .OrderBy(e => e.Date)
                                .ThenByDescending(e => e.EventType))
                                .ThenInclude(e => e.Port)
                                    .ThenInclude(p => p.Country)
                            .Include(x => x.Operator)
                            .Include(x => x.Vessel)
                                .ThenInclude(x => x.RegistrationHistory)
                                    .ThenInclude(h => h.VesselType)
                            .Include(x => x.Vessel)
                                .ThenInclude(x => x.RegistrationHistory)
                                    .ThenInclude(h => h.Flag)
                            .Include(x => x.Vessel)
                                .ThenInclude(x => x.RegistrationHistory)
                                    .ThenInclude(h => h.Operator)
                            .OrderBy(x => x.Operator.Name)
                            .ThenBy(x => x.Number)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsAsyncEnumerable();

        /// <summary>
        /// Add a voyage
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="vesselId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<Voyage> AddAsync(long operatorId, long vesselId, string number)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding voyage: Operator ID = {operatorId}, Vessel ID = {operatorId}, Number = {number}");

            // Add the voyage and save changes
            var voyage = new Voyage { OperatorId = operatorId, VesselId = vesselId, Number = number.CleanCode() };
            await Context.Voyages.AddAsync(voyage);
            await Context.SaveChangesAsync();

            // Reload to load the related entities
            voyage = await GetAsync(x => x.Id == voyage.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"Added voyage {voyage}");

            return voyage;
        }

        /// <summary>
        /// Update a voyage
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operatorId"></param>
        /// <param name="vesselId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="VoyageNotFoundException"></exception>
        public async Task<Voyage> UpdateAsync(long id, long operatorId, long vesselId, string number)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating voyage: ID = {id}, Operator ID = {operatorId}, Vessel ID = {operatorId}, Number = {number}");

            // Retrieve the voyage
            var voyage = await Context.Voyages.FirstOrDefaultAsync(x => x.Id == id);
            if (voyage == null)
            {
                var message = $"Voyage with ID {id} not found";
                throw new VoyageNotFoundException(message);
            }

            // Update the voyage properties and save changes
            voyage.OperatorId = operatorId;
            voyage.VesselId = vesselId;
            voyage.Number = number.CleanCode();
            await Context.SaveChangesAsync();

            // Reload to load the related entities
            voyage = await GetAsync(x => x.Id == voyage.Id);

            _factory.Logger.LogMessage(Severity.Debug, $"Updated voyage {voyage}");

            return voyage;
        }

        /// <summary>
        /// Delete the voyage with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="voyageNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting voyage: ID = {id}");

            // Check the voyage exists
            var voyage = await Context.Voyages.FirstOrDefaultAsync(x => x.Id == id);
            if (voyage == null)
            {
                var message = $"Voyage with ID {id} not found";
                throw new VoyageNotFoundException(message);
            }

            // Remove the associated voyage events
            var events = Context.VoyageEvents.Where(x => x.VoyageId == id);
            if (events.Any())
            {
                Context.VoyageEvents.RemoveRange(events);
                await Context.SaveChangesAsync();
            }

            // Remove the voyage
            Context.Remove(voyage);
            await Context.SaveChangesAsync();
        }
    }
}

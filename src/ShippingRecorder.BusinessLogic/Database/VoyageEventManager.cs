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
    internal class VoyageEventManager : DatabaseManagerBase, IVoyageEventManager
    {
        internal VoyageEventManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<VoyageEvent> GetAsync(Expression<Func<VoyageEvent, bool>> predicate)
        {
            List<VoyageEvent> voyageEvents = await ListAsync(predicate, 1, 1).ToListAsync();
            return voyageEvents.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<VoyageEvent> ListAsync(Expression<Func<VoyageEvent, bool>> predicate, int pageNumber, int pageSize)
            => Context.VoyageEvents
                        .Where(predicate)
                        .Include(x => x.Port)
                        .OrderBy(x => x.VoyageId)
                        .ThenBy(x => x.Date)
                        .ThenBy(x => x.EventType)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .AsAsyncEnumerable();

        /// <summary>
        /// Add a voyage event, if it doesn't already exist
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="portId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<VoyageEvent> AddAsync(long voyageId, long portId, VoyageEventType type, DateTime date)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding voyage event: Voyage ID = {voyageId}, Port ID = {portId}, Type = {type}, Date = {date}");

            // Check the voyage event doesn't already exist
            await CheckVoyageEventIsNotADuplicate(voyageId, portId, type, date, 0);

            // Add the event and save changes
            var voyageEvent = new VoyageEvent { VoyageId = voyageId, PortId = portId, EventType = type, Date = date };
            await Context.VoyageEvents.AddAsync(voyageEvent);
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(voyageEvent).Reference(x => x.Port).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added voyage event {voyageEvent}");

            return voyageEvent;
        }

        /// <summary>
        /// Add a voyage event, if it doesn't already exist
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="portId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<VoyageEvent> AddIfNotExistsAsync(long voyageId, long portId, VoyageEventType type, DateTime date)
        {
            var voyageEvent = await Match(voyageId, portId, type, date);
            if (voyageEvent == null)
            {
                voyageEvent = await AddAsync(voyageId, portId, type, date);
            }
            return voyageEvent;
        }

        /// <summary>
        /// Update a voyage event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="voyageId"></param>
        /// <param name="portId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="VoyageEventNotFoundException"></exception>
        public async Task<VoyageEvent> UpdateAsync(long id, long voyageId, long portId, VoyageEventType type, DateTime date)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating voyage event: ID = {id}, Voyage ID = {voyageId}, Port ID = {portId}, Type = {type}, Date = {date}");

            // Retrieve the voyage event
            var voyageEvent = await Context.VoyageEvents.FirstOrDefaultAsync(x => x.Id == id);
            if (voyageEvent == null)
            {
                var message = $"Voyage event with ID {id} not found";
                throw new VoyageEventNotFoundException(message);
            }

            // Check the event doesn't already exist
            await CheckVoyageEventIsNotADuplicate(voyageId, portId, type, date, id);

            // Update the event properties and save changes
            voyageEvent.VoyageId = voyageId;
            voyageEvent.PortId = portId;
            voyageEvent.EventType = type;
            voyageEvent.Date = date;
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(voyageEvent).Reference(x => x.Port).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated voyage event {voyageEvent}");

            return voyageEvent;
        }

        /// <summary>
        /// Delete the voyage event with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="voyageNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting voyage event: ID = {id}");

            // Check the voyage event exists
            var voyage = await Context.VoyageEvents.FirstOrDefaultAsync(x => x.Id == id);
            if (voyage == null)
            {
                var message = $"Voyage event with ID {id} not found";
                throw new VoyageEventNotFoundException(message);
            }

            // Remove the voyage
            Context.Remove(voyage);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate voyage event
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="portId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="VoyageEventExistsException"></exception>
        private async Task CheckVoyageEventIsNotADuplicate(long voyageId, long portId, VoyageEventType type, DateTime date, long id)
        {
            var voyage = await Match(voyageId, portId, type, date);
            if ((voyage != null) && (voyage.Id != id))
            {
                var message = $"Voyage event {type} already exists for port ID {portId} and voyage ID {voyageId}";
                throw new VoyageEventExistsException(message);
            }
        }

        /// <summary>
        /// Find the first event matching the specified criteria
        /// </summary>
        /// <param name="voyageId"></param>
        /// <param name="portId"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private async Task<VoyageEvent> Match(long voyageId, long portId, VoyageEventType type, DateTime date)
            => await Context.VoyageEvents.FirstOrDefaultAsync(x =>
                                                    (x.VoyageId == voyageId) &&
                                                    (x.PortId == portId) &&
                                                    (x.EventType == type) &&
                                                    (x.Date.Ticks == date.Ticks));
    }
}

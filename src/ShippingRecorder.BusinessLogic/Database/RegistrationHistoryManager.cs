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
    internal class RegistrationHistoryManager : DatabaseManagerBase, IRegistrationHistoryManager
    {

        internal RegistrationHistoryManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<RegistrationHistory> GetAsync(Expression<Func<RegistrationHistory, bool>> predicate)
        {
            List<RegistrationHistory> registrations = await ListAsync(predicate, 1, 1).ToListAsync();
            return registrations.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<RegistrationHistory> ListAsync(Expression<Func<RegistrationHistory, bool>> predicate, int pageNumber, int pageSize)
            => Context.RegistrationHistory
                            .Where(predicate)
                            .Include(x => x.Flag)
                            .Include(x => x.Operator)
                            .Include(x => x.VesselType)
                            .OrderBy(x => x.VesselId)
                            .ThenBy(x => x.Date)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsAsyncEnumerable();

        /// <summary>
        /// Add a vessel registration history
        /// </summary>
        /// <param name="vesselId"></param>
        /// <param name="vesselTypeId"></param>
        /// <param name="flagId"></param>
        /// <param name="operatorId"></param>
        /// <param name="name"></param>
        /// <param name="callsign"></param>
        /// <param name="mmsi"></param>
        /// <param name="tonnage"></param>
        /// <param name="passengers"></param>
        /// <param name="crew"></param>
        /// <param name="decks"></param>
        /// <param name="cabins"></param>
        /// <returns></returns>
        public async Task<RegistrationHistory> AddAsync(
            long vesselId,
            long vesselTypeId,
            long flagId,
            long operatorId,
            string name,
            string callsign,
            string mmsi,
            int? tonnage,
            int? passengers,
            int? crew,
            int? decks,
            int? cabins)
        {
            callsign = callsign.CleanCode();
            mmsi = mmsi.CleanCode();

            _factory.Logger.LogMessage(Severity.Debug,
                $"Adding registration: " +
                $"Vessel ID = {vesselId}, " +
                $"Vessel Type ID = {vesselTypeId}, " +
                $"Flag ID = {flagId}, " +
                $"Operator ID = {operatorId}, " +
                $"Name = {name}, " +
                $"Callsign = {callsign}, " +
                $"MMSI = {mmsi}");

            // Validate the MMSI
            mmsi.ValidateNumericAndThrow<InvalidMMSIException>(9, 9);
    
            // Add the details and save changes
            var registration = new RegistrationHistory
            {
                VesselId = vesselId,
                VesselTypeId = vesselTypeId,
                FlagId = flagId,
                OperatorId = operatorId,
                Name = name,
                Callsign = callsign,
                MMSI = mmsi,
                Tonnage = tonnage,
                Passengers = passengers,
                Crew = crew,
                Decks = decks,
                Cabins = cabins,
                Date = DateTime.Now,
                IsActive = true
            };
            await Context.RegistrationHistory.AddAsync(registration);
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(registration).Reference(x => x.Flag).LoadAsync();
            await Context.Entry(registration).Reference(x => x.VesselType).LoadAsync();
            await Context.Entry(registration).Reference(x => x.Operator).LoadAsync();

            // Find previous active entries and disable them
            var previousRegistrations = Context.RegistrationHistory.Where(x =>
                                                    (x.Id != registration.Id) &&
                                                    (x.VesselId == vesselId) &&
                                                    x.IsActive);
            foreach (var previous in previousRegistrations)
            {
                previous.IsActive = false;
            }
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added registration {registration}");

            return registration;
        }

        /// <summary>
        /// Disable the registration record with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="locationNotFoundException"></exception>
        public async Task Deactivate(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting registration history: ID = {id}");

            // Check the location exists
            var registration = await Context.RegistrationHistory.FirstOrDefaultAsync(x => x.Id == id);
            if (registration == null)
            {
                var message = $"Registration history with ID {id} not found";
                throw new RegistrationHistoryNotFoundException(message);
            }

            // Deactivate the specified registration record
            registration.IsActive = false;
            await Context.SaveChangesAsync();
        }
    }
}

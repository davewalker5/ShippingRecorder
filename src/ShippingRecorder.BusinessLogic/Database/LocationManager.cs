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
    internal class LocationManager : DatabaseManagerBase, ILocationManager
    {

        internal LocationManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Location> GetAsync(Expression<Func<Location, bool>> predicate)
        {
            List<Location> locations = await ListAsync(predicate, 1, 1).ToListAsync();
            return locations.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize)
            => Context.Locations
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsAsyncEnumerable();

        /// <summary>
        /// Add a location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding location: Name = {name}");

            // Check the location doesn't already exist
            name = name.TitleCase();
            await CheckLocationIsNotADuplicate(name, 0);

            // Add the location and save changes
            var location = new Location { Name = name };
            await Context.Locations.AddAsync(location);
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added location {location}");

            return location;
        }

        /// <summary>
        /// Add a location, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> AddIfNotExistsAsync(string name)
        {
            name = name.TitleCase();
            var location = await GetAsync(x => x.Name == name);
            if (location == null)
            {
                location = await AddAsync(name);
            }
            return location;
        }

        /// <summary>
        /// Update a location
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Location> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating location: ID = {id}, Name = {name}");

            // Retrieve the location
            var location = await Context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (location == null)
            {
                var message = $"Location with ID {id} not found";
                throw new LocationNotFoundException(message);
            }

            // Check the location doesn't already exist
            name = name.TitleCase();            
            await CheckLocationIsNotADuplicate(name, id);

            // Update the location properties and save changes
            location.Name = name;
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated location {location}");

            return location;
        }

        /// <summary>
        /// Delete the location with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="locationNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting location: ID = {id}");

            // Check the location exists
            var location = await Context.Locations.FirstOrDefaultAsync(x => x.Id == id);
            if (location == null)
            {
                var message = $"Location with ID {id} not found";
                throw new LocationNotFoundException(message);
            }

            // Remove the location
            Context.Remove(location);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate location
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="LocationExistsException"></exception>
        private async Task CheckLocationIsNotADuplicate(string name, long id)
        {
            var location = await Context.Locations.FirstOrDefaultAsync(x => x.Name == name);
            if ((location != null) && (location.Id != id))
            {
                var message = $"Location {name} already exists";
                throw new LocationExistsException(message);
            }
        }
    }
}

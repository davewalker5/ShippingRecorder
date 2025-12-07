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
    internal class VesselTypeManager : DatabaseManagerBase, IVesselTypeManager
    {

        internal VesselTypeManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<VesselType> GetAsync(Expression<Func<VesselType, bool>> predicate)
        {
            List<VesselType> vesselTypes = await ListAsync(predicate, 1, 1).ToListAsync();
            return vesselTypes.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<VesselType> ListAsync(Expression<Func<VesselType, bool>> predicate, int pageNumber, int pageSize)
            => Context.VesselTypes
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsAsyncEnumerable();

        /// <summary>
        /// Add a vessel type, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<VesselType> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding vessel type: Name = {name}");

            // Check the vessel type doesn't already exist
            name = name.TitleCase();
            await CheckVesselTypeIsNotADuplicate(name, 0);

            // Add the vessel type and save changes
            var vesselType = new VesselType { Name = name };
            await Context.VesselTypes.AddAsync(vesselType);
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added vessel type {vesselType}");

            return vesselType;
        }

        /// <summary>
        /// Add a vessel type, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<VesselType> AddIfNotExistsAsync(string name)
        {
            name = name.TitleCase();
            var vesselType = await GetAsync(x => x.Name == name);
            if (vesselType == null)
            {
                vesselType = await AddAsync(name);
            }
            return vesselType;
        }

        /// <summary>
        /// Update a vessel type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<VesselType> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating vessel type: ID = {id}, Name = {name}");

            // Retrieve the vessel type
            var vesselType = await Context.VesselTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (vesselType == null)
            {
                var message = $"Vessel type with ID {id} not found";
                throw new VesselTypeNotFoundException(message);
            }

            // Check the vessel type doesn't already exist
            name = name.TitleCase();            
            await CheckVesselTypeIsNotADuplicate(name, id);

            // Update the vessel type properties and save changes
            vesselType.Name = name;
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated vessel type {vesselType}");

            return vesselType;
        }

        /// <summary>
        /// Delete the vessel type with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="vesselTypeNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting vessel type: ID = {id}");

            // Check the vesselType exists
            var vesselType = await Context.VesselTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (vesselType == null)
            {
                var message = $"Vessel type with ID {id} not found";
                throw new VesselTypeNotFoundException(message);
            }

            // Remove the vessel type
            Context.Remove(vesselType);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate vessel type
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="VesselTypeExistsException"></exception>
        private async Task CheckVesselTypeIsNotADuplicate(string name, long id)
        {
            var vesselType = await Context.VesselTypes.FirstOrDefaultAsync(x => x.Name == name);
            if ((vesselType != null) && (vesselType.Id != id))
            {
                var message = $"Vessel type {name} already exists";
                throw new VesselTypeExistsException(message);
            }
        }
    }
}

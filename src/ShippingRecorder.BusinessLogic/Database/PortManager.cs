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
    internal class PortManager : DatabaseManagerBase, IPortManager
    {
        internal PortManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Port> GetAsync(Expression<Func<Port, bool>> predicate)
        {
            List<Port> countries = await ListAsync(predicate, 1, 1).ToListAsync();
            return countries.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Port> ListAsync(Expression<Func<Port, bool>> predicate, int pageNumber, int pageSize)
            => Context.Ports
                        .Where(predicate)
                        .Include(x => x.Country)
                        .OrderBy(x => x.Name)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .AsAsyncEnumerable();

        /// <summary>
        /// Add a port, if it doesn't already exist
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Port> AddAsync(long countryId, string code, string name)
        {
            code = code.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Adding port: Country ID = {countryId}, Code = {code}, Name = {name}");

            // Check the port doesn't already exist
            await CheckPortIsNotADuplicate(code, 0);

            // Add the port and save changes
            var port = new Port { CountryId = countryId, Code = code, Name = name };
            await Context.Ports.AddAsync(port);
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(port).Reference(x => x.Country).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added port {port}");

            return port;
        }

        /// <summary>
        /// Add a port, if it doesn't already exist
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Port> AddIfNotExistsAsync(long countryId, string code, string name)
        {
            code = code.CleanCode();
            var port = await GetAsync(x => x.Code == code);
            if (port == null)
            {
                port = await AddAsync(countryId, code, name);
            }
            return port;
        }

        /// <summary>
        /// Update a port
        /// </summary>
        /// <param name="id"></param>
        /// <param name="countryId"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Port> UpdateAsync(long id, long countryId, string code, string name)
        {
            code = code.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Updating port: ID = {id}, Country ID = {countryId}, Code = {code}, Name = {name}");

            // Retrieve the port
            var port = await Context.Ports.FirstOrDefaultAsync(x => x.Id == id);
            if (port == null)
            {
                var message = $"Port with ID {id} not found";
                throw new PortNotFoundException(message);
            }

            // Check the port doesn't already exist
            await CheckPortIsNotADuplicate(code, id);

            // Update the port properties and save changes
            port.CountryId = countryId;
            port.Code = code;
            port.Name = name;
            await Context.SaveChangesAsync();

            // Load related entities
            await Context.Entry(port).Reference(x => x.Country).LoadAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated port {port}");

            return port;
        }

        /// <summary>
        /// Delete the port with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="portNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting port: ID = {id}");

            // Check the port exists
            var port = await Context.Ports.FirstOrDefaultAsync(x => x.Id == id);
            if (port == null)
            {
                var message = $"Port with ID {id} not found";
                throw new PortNotFoundException(message);
            }

            // Remove the port
            Context.Remove(port);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate port
        /// </summary>
        /// <param code="code"></param>
        /// <param name="id"></param>
        /// <exception cref="PortExistsException"></exception>
        private async Task CheckPortIsNotADuplicate(string code, long id)
        {
            var port = await Context.Ports.FirstOrDefaultAsync(x => x.Code == code);
            if ((port != null) && (port.Id != id))
            {
                var message = $"Port {code} already exists";
                throw new PortExistsException(message);
            }
        }
    }
}

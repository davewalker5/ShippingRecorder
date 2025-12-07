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
    internal class CountryManager : DatabaseManagerBase, ICountryManager
    {

        internal CountryManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Country> GetAsync(Expression<Func<Country, bool>> predicate)
        {
            List<Country> countries = await ListAsync(predicate, 1, 1).ToListAsync();
            return countries.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Country> ListAsync(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize)
            => Context.Countries
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsAsyncEnumerable();

        /// <summary>
        /// Add a country, if it doesn't already exist
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> AddAsync(string code, string name)
        {
            code = code.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Adding country: Code = {code}, Name = {name}");

            // Validate the country code
            code.ValidateAlphaAndThrow<InvalidCountryCodeException>(2, 2);

            // Check the country doesn't already exist
            await CheckCountryIsNotADuplicate(code, 0);

            // Add the country and save changes
            var country = new Country { Code = code, Name = name };
            await Context.Countries.AddAsync(country);
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added country {country}");

            return country;
        }

        /// <summary>
        /// Add a country, if it doesn't already exist
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> AddIfNotExistsAsync(string code, string name)
        {
            code = code.CleanCode();
            var country = await GetAsync(x => x.Code == code);
            if (country == null)
            {
                country = await AddAsync(code, name);
            }
            return country;
        }

        /// <summary>
        /// Update a country
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Country> UpdateAsync(long id, string code, string name)
        {
            code = code.CleanCode();
            _factory.Logger.LogMessage(Severity.Debug, $"Updating country: ID = {id}, Code = {code}, Name = {name}");

            // Validate the country code
            code.ValidateAlphaAndThrow<InvalidCountryCodeException>(2, 2);

            // Retrieve the country
            var country = await Context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                var message = $"Country with ID {id} not found";
                throw new CountryNotFoundException(message);
            }

            // Check the country doesn't already exist
            await CheckCountryIsNotADuplicate(code, id);

            // Update the country properties and save changes
            country.Code = code;
            country.Name = name;
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated country {country}");

            return country;
        }

        /// <summary>
        /// Delete the country with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="countryNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting country: ID = {id}");

            // Check the country exists
            var country = await Context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                var message = $"Country with ID {id} not found";
                throw new CountryNotFoundException(message);
            }

            // Remove the country
            Context.Remove(country);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate country
        /// </summary>
        /// <param code="code"></param>
        /// <param name="id"></param>
        /// <exception cref="CountryExistsException"></exception>
        private async Task CheckCountryIsNotADuplicate(string code, long id)
        {
            var country = await Context.Countries.FirstOrDefaultAsync(x => x.Code == code);
            if ((country != null) && (country.Id != id))
            {
                var message = $"Country {code} already exists";
                throw new CountryExistsException(message);
            }
        }
    }
}

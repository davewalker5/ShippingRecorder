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
    internal class OperatorManager : DatabaseManagerBase, IOperatorManager
    {

        internal OperatorManager(IShippingRecorderFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Return the first entity matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<Operator> GetAsync(Expression<Func<Operator, bool>> predicate)
        {
            List<Operator> operators = await ListAsync(predicate, 1, 1).ToListAsync();
            return operators.FirstOrDefault();
        }

        /// <summary>
        /// Return all entities matching the specified criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IAsyncEnumerable<Operator> ListAsync(Expression<Func<Operator, bool>> predicate, int pageNumber, int pageSize)
            => Context.Operators
                            .Where(predicate)
                            .OrderBy(x => x.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .AsAsyncEnumerable();

        /// <summary>
        /// Add an operator, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Operator> AddAsync(string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Adding operator: Name = {name}");

            // Check the operator doesn't already exist
            name = name.TitleCase();
            await CheckOperatorIsNotADuplicate(name, 0);

            // Add the operator and save changes
            var op = new Operator { Name = name };
            await Context.Operators.AddAsync(op);
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Added operator {op}");

            return op;
        }

        /// <summary>
        /// Add an operator, if it doesn't already exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Operator> AddIfNotExistsAsync(string name)
        {
            name = name.TitleCase();
            var op = await GetAsync(x => x.Name == name);
            if (op == null)
            {
                op = await AddAsync(name);
            }
            return op;
        }

        /// <summary>
        /// Update a operator
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Operator> UpdateAsync(long id, string name)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Updating operator: ID = {id}, Name = {name}");

            // Retrieve the operator
            var op = await Context.Operators.FirstOrDefaultAsync(x => x.Id == id);
            if (op == null)
            {
                var message = $"Operator with ID {id} not found";
                throw new OperatorNotFoundException(message);
            }

            // Check the operator doesn't already exist
            name = name.TitleCase();            
            await CheckOperatorIsNotADuplicate(name, id);

            // Update the operator properties and save changes
            op.Name = name;
            await Context.SaveChangesAsync();

            _factory.Logger.LogMessage(Severity.Debug, $"Updated operator {op}");

            return op;
        }

        /// <summary>
        /// Delete the operator with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="operatorNotFoundException"></exception>
        public async Task DeleteAsync(long id)
        {
            _factory.Logger.LogMessage(Severity.Debug, $"Deleting operator: ID = {id}");

            // Check the operator exists
            var op = await Context.Operators.FirstOrDefaultAsync(x => x.Id == id);
            if (op == null)
            {
                var message = $"Operator with ID {id} not found";
                throw new OperatorNotFoundException(message);
            }

            // Remove the operator
            Context.Remove(op);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Raise an exception if an attempt is made to add/update a duplicate operator
        /// </summary>
        /// <param code="name"></param>
        /// <param name="id"></param>
        /// <exception cref="OperatorExistsException"></exception>
        private async Task CheckOperatorIsNotADuplicate(string name, long id)
        {
            var op = await Context.Operators.FirstOrDefaultAsync(x => x.Name == name);
            if ((op != null) && (op.Id != id))
            {
                var message = $"Operator {name} already exists";
                throw new OperatorExistsException(message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IOperatorManager
    {
        Task<Operator> AddAsync(string name);
        Task<Operator> AddIfNotExistsAsync(string name);
        Task<Operator> GetAsync(Expression<Func<Operator, bool>> predicate);
        IAsyncEnumerable<Operator> ListAsync(Expression<Func<Operator, bool>> predicate, int pageNumber, int pageSize);
        Task<Operator> UpdateAsync(long id, string name);
        Task DeleteAsync(long id);
    }
}
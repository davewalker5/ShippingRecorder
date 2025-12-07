using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IPortManager
    {
        Task<Port> AddAsync(long countryId, string code, string name);
        Task<Port> AddIfNotExistsAsync(long countryId, string code, string name);
        Task<Port> GetAsync(Expression<Func<Port, bool>> predicate);
        IAsyncEnumerable<Port> ListAsync(Expression<Func<Port, bool>> predicate, int pageNumber, int pageSize);
        Task<Port> UpdateAsync(long id, long countryId, string code, string name);
        Task DeleteAsync(long id);
    }
}
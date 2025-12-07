using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface ILocationManager
    {
        Task<Location> AddAsync(string name);
        Task<Location> AddIfNotExistsAsync(string name);
        Task<Location> GetAsync(Expression<Func<Location, bool>> predicate);
        IAsyncEnumerable<Location> ListAsync(Expression<Func<Location, bool>> predicate, int pageNumber, int pageSize);
        Task<Location> UpdateAsync(long id, string name);
        Task DeleteAsync(long id);
    }
}
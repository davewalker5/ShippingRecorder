using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface ICountryManager
    {
        Task<Country> AddAsync(string code, string name);
        Task<Country> AddIfNotExistsAsync(string code, string name);
        Task<Country> GetAsync(Expression<Func<Country, bool>> predicate);
        IAsyncEnumerable<Country> ListAsync(Expression<Func<Country, bool>> predicate, int pageNumber, int pageSize);
        Task<Country> UpdateAsync(long id, string code, string name);
        Task DeleteAsync(long id);
    }
}
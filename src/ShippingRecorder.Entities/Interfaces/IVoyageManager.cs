using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IVoyageManager
    {
        Task<Voyage> AddAsync(long operatorId, long vesselId, string number);
        Task<Voyage> GetAsync(Expression<Func<Voyage, bool>> predicate);
        IAsyncEnumerable<Voyage> ListAsync(Expression<Func<Voyage, bool>> predicate, int pageNumber, int pageSize);
        Task<Voyage> UpdateAsync(long id, long operatorId, long vesselId, string number);
        Task DeleteAsync(long id);
    }
}
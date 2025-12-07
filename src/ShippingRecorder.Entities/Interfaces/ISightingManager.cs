using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface ISightingManager
    {
        Task<Sighting> GetAsync(Expression<Func<Sighting, bool>> predicate);
        IAsyncEnumerable<Sighting> ListAsync(Expression<Func<Sighting, bool>> predicate, int pageNumber, int pageSize);
        Task<Sighting> AddAsync(long locationId, long? voyageId, long vesselId, DateTime date, bool isMyVoyage);
        Task<Sighting> UpdateAsync(long id, long locationId, long? voyageId, long vesselId, DateTime date, bool isMyVoyage);
        Task DeleteAsync(long id);
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IVoyageEventManager
    {
        Task<VoyageEvent> GetAsync(Expression<Func<VoyageEvent, bool>> predicate);
        IAsyncEnumerable<VoyageEvent> ListAsync(Expression<Func<VoyageEvent, bool>> predicate, int pageNumber, int pageSize);
        Task<VoyageEvent> AddAsync(long voyageId, long portId, VoyageEventType type, DateTime date);
        Task<VoyageEvent> AddIfNotExistsAsync(long voyageId, long portId, VoyageEventType type, DateTime date);
        Task<VoyageEvent> UpdateAsync(long id, long voyageId, long portId, VoyageEventType type, DateTime date);
        Task DeleteAsync(long id);
    }
}
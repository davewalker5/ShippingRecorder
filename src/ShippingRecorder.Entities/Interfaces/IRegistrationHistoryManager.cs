using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IRegistrationHistoryManager
    {
        Task<RegistrationHistory> GetAsync(Expression<Func<RegistrationHistory, bool>> predicate);
        IAsyncEnumerable<RegistrationHistory> ListAsync(Expression<Func<RegistrationHistory, bool>> predicate, int pageNumber, int pageSize);
        Task<RegistrationHistory> AddAsync(
            long vesselId,
            long vesselTypeId,
            long flagId,
            long operatorId,
            string name,
            string callsign,
            string mmsi,
            int? tonnage,
            int? passengers,
            int? crew,
            int? decks,
            int? cabins);
        Task Deactivate(long id);
    }
}
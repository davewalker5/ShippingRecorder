using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IVoyageEventClient
    {
        Task<VoyageEvent> AddAsync(long voyageId, VoyageEventType eventType, long portId, DateTime date);
        Task DeleteAsync(long id);
        Task<List<VoyageEvent>> ListAsync(long voyageId, int pageNumber, int pageSize);
        Task<VoyageEvent> UpdateAsync(long id, long voyageId, VoyageEventType eventType, long portId, DateTime date);
    }
}
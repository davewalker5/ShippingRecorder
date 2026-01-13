using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IVoyageClient
    {
        Task<Voyage> GetAsync(long id);
        Task<Voyage> AddAsync(long operatorId, long vesselId, string number);
        Task DeleteAsync(long id);
        Task<List<Voyage>> ListAsync(long operatorId, int pageNumber, int pageSize);
        Task<Voyage> UpdateAsync(long id, long operatorId, long vesselId, string number);
    }
}
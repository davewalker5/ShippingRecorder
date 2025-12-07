using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IVoyageClient
    {
        Task<Voyage> AddAsync(long operatorId, string number);
        Task DeleteAsync(long id);
        Task<List<Voyage>> ListAsync(int pageNumber, int pageSize);
        Task<Voyage> UpdateAsync(long id, long operatorId, string number);
    }
}
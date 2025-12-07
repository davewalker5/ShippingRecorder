using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IVesselTypeClient
    {
        Task<VesselType> AddAsync(string name);
        Task DeleteAsync(long id);
        Task<List<VesselType>> ListAsync(int pageNumber, int pageSize);
        Task<VesselType> UpdateAsync(long id, string name);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface ILocationClient : IImporter, IExporter
    {
        Task<Location> GetAsync(long id);
        Task<Location> AddAsync(string name);
        Task DeleteAsync(long id);
        Task<List<Location>> ListAsync(int pageNumber, int pageSize);
        Task<Location> UpdateAsync(long id, string name);
        
    }
}
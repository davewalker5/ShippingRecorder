using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IPortClient : IPortsRetriever, IImporter, IExporter
    {
        Task<Port> AddAsync(long countryId, string code, string name);
        Task DeleteAsync(long id);
        Task<List<Port>> ListAsync(long countryId, int pageNumber, int pageSize);
        Task<Port> UpdateAsync(long id, long countryId, string code, string name);
    }
}
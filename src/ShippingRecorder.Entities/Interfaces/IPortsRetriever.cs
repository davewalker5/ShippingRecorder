using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IPortsRetriever
    {
        Task<List<Port>> GetPortsAsync(int pageNumber, int pageSize);
    }
}
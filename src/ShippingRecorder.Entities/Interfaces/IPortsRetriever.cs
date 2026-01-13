using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IPortsRetriever
    {
        Task<Port> GetAsync(string code);
    }
}
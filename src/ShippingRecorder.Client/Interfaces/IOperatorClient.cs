using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IOperatorClient : IImporter, IExporter
    {
        Task<Operator> GetAsync(long id);
        Task<Operator> AddAsync(string name);
        Task DeleteAsync(long id);
        Task<List<Operator>> ListAsync(int pageNumber, int pageSize);
        Task<Operator> UpdateAsync(long id, string name);
    }
}
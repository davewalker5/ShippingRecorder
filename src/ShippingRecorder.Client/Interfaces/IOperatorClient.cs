using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IOperatorClient
    {
        Task<Operator> AddAsync(string name);
        Task DeleteAsync(long id);
        Task<List<Operator>> ListAsync(int pageNumber, int pageSize);
        Task<Operator> UpdateAsync(long id, string name);
    }
}
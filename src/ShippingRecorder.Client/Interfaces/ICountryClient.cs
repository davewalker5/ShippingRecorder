using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface ICountryClient : IImporter, IExporter
    {
        Task<Country> GetAsync(long id);
        Task<Country> AddAsync(string code, string name);
        Task DeleteAsync(long id);
        Task<List<Country>> ListAsync(int pageNumber, int pageSize);
        Task<Country> UpdateAsync(long id, string code, string name);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IVesselClient : IImporter, IExporter
    {
        Task<Vessel> GetAsync(long id);
        Task<Vessel> GetAsync(string imo);
        Task<Vessel> AddAsync(string imo, int? built, decimal? draught, int? length, int? beam);
        Task DeleteAsync(long id);
        Task<List<Vessel>> ListAsync(int pageNumber, int pageSize);
        Task<Vessel> UpdateAsync(long id, string imo, int? built, decimal? draught, int? length, int? beam);
    }
}
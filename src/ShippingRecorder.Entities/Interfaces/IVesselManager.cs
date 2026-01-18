using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IVesselManager
    {
        Task<Vessel> AddAsync(string identifier, bool isIMO, int? built, decimal? draught, int? length, int? beam);
        Task<Vessel> AddIfNotExistsAsync(string identifier, bool isIMO, int? built, decimal? draught, int? length, int? beam);
        Task<Vessel> GetAsync(Expression<Func<Vessel, bool>> predicate);
        IAsyncEnumerable<Vessel> ListAsync(Expression<Func<Vessel, bool>> predicate, int pageNumber, int pageSize);
        Task<Vessel> UpdateAsync(long id, string identifier, bool isIMO, int? built, decimal? draught, int? length, int? beam);
        Task DeleteAsync(long id);
    }
}
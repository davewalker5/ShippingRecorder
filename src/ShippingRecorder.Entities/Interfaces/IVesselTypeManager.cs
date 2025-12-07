using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IVesselTypeManager
    {
        Task<VesselType> AddAsync(string name);
        Task<VesselType> AddIfNotExistsAsync(string name);
        Task<VesselType> GetAsync(Expression<Func<VesselType, bool>> predicate);
        IAsyncEnumerable<VesselType> ListAsync(Expression<Func<VesselType, bool>> predicate, int pageNumber, int pageSize);
        Task<VesselType> UpdateAsync(long id, string name);
        Task DeleteAsync(long id);
    }
}
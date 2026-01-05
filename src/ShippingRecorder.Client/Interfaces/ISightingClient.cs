using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface ISightingClient
    {
        Task<Sighting> GetAsync(long id);
        Task<Sighting> AddAsync(long locationId, long? voyageId, long sightingId, DateTime date, bool isMyVoyage);
        Task DeleteAsync(long id);
        Task<List<Sighting>> ListAsync(int pageNumber, int pageSize);
        Task<List<Sighting>> ListSightingsByVesselAsync(long vesselId, int pageNumber, int pageSize);
        Task<List<Sighting>> ListSightingsByLocationAsync(long locationId, int pageNumber, int pageSize);
        Task<List<Sighting>> ListSightingsByDateAsync(DateTime start, DateTime end,  int pageNumber, int pageSize);
        Task<List<Sighting>> ListMyVoyagesAsync(int pageNumber, int pageSize);
        Task<Sighting> UpdateAsync(long id, long locationId, long? voyageId, long sightingId, DateTime date, bool isMyVoyage);
    }
}
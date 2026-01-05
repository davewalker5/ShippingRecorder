using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IRegistrationHistoryClient
    {
        Task<RegistrationHistory> GetActiveRegistrationForVesselAsync(long vesselId);

        Task<RegistrationHistory> AddAsync(
            long vesselId,
            long vesselTypeId,
            long flagId,
            long operatorId,
            DateTime date,
            string name,
            string callsign,
            string mmsi,
            int? tonnage,
            int? passengers,
            int? crew,
            int? decks,
            int? cabins);

        Task Deactivate(long id);
        Task<List<RegistrationHistory>> ListAsync(long vesselId, int pageNumber, int pageSize);
    }
}
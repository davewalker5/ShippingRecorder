using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface ISightingSearchClient
    {
        Task<Sighting> GetMostRecentVesselSightingAsync(string imo);
    }
}
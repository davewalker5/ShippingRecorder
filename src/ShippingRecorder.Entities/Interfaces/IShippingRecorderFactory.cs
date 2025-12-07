using Microsoft.EntityFrameworkCore;

namespace ShippingRecorder.Entities.Interfaces
{
    public interface IShippingRecorderFactory
    {
        IShippingRecorderLogger Logger { get; }
        ILocationManager Locations { get; }
        IOperatorManager Operators { get; }
        IVesselTypeManager VesselTypes { get; }
        ICountryManager Countries { get; }
        IPortManager Ports { get; }
        IVesselManager Vessels { get; }
        IVoyageManager Voyages { get; }
        IVoyageEventManager VoyageEvents { get; }
        IRegistrationHistoryManager RegistrationHistory { get; }
        ISightingManager Sightings { get; }
        IUserManager Users { get; }
        T GetContext<T>() where T : DbContext;
    }
}
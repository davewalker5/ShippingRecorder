using Microsoft.EntityFrameworkCore;
using ShippingRecorder.Entities.Reporting;

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
        IJobStatusManager JobStatuses { get; }
        T GetContext<T>() where T : DbContext;

        IDateBasedReport<LocationStatistics> LocationStatistics { get; }
        IDateBasedReport<SightingsByMonth> SightingsByMonth { get; }
        IDateBasedReport<MyVoyages> MyVoyages { get; }
        IDateBasedReport<OperatorStatistics> OperatorStatistics { get; }
        IDateBasedReport<VesselTypeStatistics> VesselTypeStatistics { get; }
        IDateBasedReport<FlagStatistics> FlagStatistics { get; }
    }
}
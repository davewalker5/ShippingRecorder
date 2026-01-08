using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ShippingRecorder.BusinessLogic.Database;
using ShippingRecorder.Data;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Reporting;

namespace ShippingRecorder.BusinessLogic.Factory
{
    public class ShippingRecorderFactory : IShippingRecorderFactory
    {
        private DbContext _context;
        private readonly Lazy<ILocationManager> _locations = null;
        private readonly Lazy<IOperatorManager> _operators = null;
        private readonly Lazy<IVesselTypeManager> _vesselTypes = null;
        private readonly Lazy<ICountryManager> _countries = null;
        private readonly Lazy<IPortManager> _ports = null;
        private readonly Lazy<IVesselManager> _vessels = null;
        private readonly Lazy<IVoyageManager> _voyages = null;
        private readonly Lazy<IVoyageEventManager> _voyageEvents = null;
        private readonly Lazy<IRegistrationHistoryManager> _registrationHistory = null;
        private readonly Lazy<ISightingManager> _sightings = null;
        private readonly Lazy<IUserManager> _users = null;
        private readonly Lazy<IJobStatusManager> _jobStatuses = null;

        private readonly Lazy<IDateBasedReport<LocationStatistics>> _locationStatistics = null;
        private readonly Lazy<IDateBasedReport<SightingsByMonth>> _sightingsByMonth = null;
        private readonly Lazy<IDateBasedReport<MyVoyages>> _myVoyages = null;
        private readonly Lazy<IDateBasedReport<OperatorStatistics>> _operatorStatistics = null;

        public IShippingRecorderLogger Logger { get; private set; }
        public ILocationManager Locations { get { return _locations.Value; } }
        public IOperatorManager Operators { get { return _operators.Value; } }
        public IVesselTypeManager VesselTypes { get { return _vesselTypes.Value; } }
        public ICountryManager Countries { get { return _countries.Value; } }
        public IPortManager Ports { get { return _ports.Value; } }
        public IVesselManager Vessels { get { return _vessels.Value; } }
        public IVoyageManager Voyages { get { return _voyages.Value; } }
        public IVoyageEventManager VoyageEvents { get { return _voyageEvents.Value; } }
        public IRegistrationHistoryManager RegistrationHistory { get { return _registrationHistory.Value; } }
        public ISightingManager Sightings { get { return _sightings.Value; } }
        public IUserManager Users { get { return _users.Value; } }
        public IJobStatusManager JobStatuses { get { return _jobStatuses.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<LocationStatistics> LocationStatistics { get { return _locationStatistics.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<SightingsByMonth> SightingsByMonth { get { return _sightingsByMonth.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<MyVoyages> MyVoyages { get { return _myVoyages.Value; } }

        [ExcludeFromCodeCoverage]
        public IDateBasedReport<OperatorStatistics> OperatorStatistics { get { return _operatorStatistics.Value; } }

        public ShippingRecorderFactory(ShippingRecorderDbContext context, IShippingRecorderLogger logger)
        {
            // Store the database context and logger
            _context = context;
            Logger = logger;

            // Lazily instantiate the database managers : They'll only actually be created if called by
            // the application
            _locations = new Lazy<ILocationManager>(() => new LocationManager(this));
            _operators = new Lazy<IOperatorManager>(() => new OperatorManager(this));
            _vesselTypes = new Lazy<IVesselTypeManager>(() => new VesselTypeManager(this));
            _countries = new Lazy<ICountryManager>(() => new CountryManager(this));
            _ports = new Lazy<IPortManager>(() => new PortManager(this));
            _vessels = new Lazy<IVesselManager>(() => new VesselManager(this));
            _voyages = new Lazy<IVoyageManager>(() => new VoyageManager(this));
            _voyageEvents = new Lazy<IVoyageEventManager>(() => new VoyageEventManager(this));
            _registrationHistory = new Lazy<IRegistrationHistoryManager>(() => new RegistrationHistoryManager(this));
            _sightings = new Lazy<ISightingManager>(() => new SightingManager(this));
            _users = new Lazy<IUserManager>(() => new UserManager(this));
            _jobStatuses = new Lazy<IJobStatusManager>(() => new JobStatusManager(this));

            // Lazily instantiate the reporting managers : Once again, they'll only actually be created if called by
            // the application
            _locationStatistics = new Lazy<IDateBasedReport<LocationStatistics>>(() => new DateBasedReport<LocationStatistics>(this));
            _sightingsByMonth = new Lazy<IDateBasedReport<SightingsByMonth>>(() => new DateBasedReport<SightingsByMonth>(this));
            _myVoyages = new Lazy<IDateBasedReport<MyVoyages>>(() => new DateBasedReport<MyVoyages>(this));
            _operatorStatistics = new Lazy<IDateBasedReport<OperatorStatistics>>(() => new DateBasedReport<OperatorStatistics>(this));
        }

        /// <summary>
        /// Return the database context as a DbContext of type "T"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetContext<T>() where T : DbContext
            => _context as T;
    }
}

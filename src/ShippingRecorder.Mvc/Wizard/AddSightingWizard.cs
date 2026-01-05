using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;

namespace ShippingRecorder.Mvc.Wizard
{
    public class AddSightingWizard
    {
        private const string SightingDetailsKeyPrefix = "Wizard.SightingDetails";
        private const string VesselDetailsKeyPrefix = "Wizard.VesselDetails";
        private const string LastSightingAddedKeyPrefix = "Wizard.LastSightingAdded";
        private const string DefaultDateKeyPrefix = "Wizard.DefaultDate";

        private readonly ILocationClient _locationClient;
        private readonly IVesselTypeListGenerator _vesselTypeListGenerator;
        private readonly ICountryListGenerator _countryListGenerator;
        private readonly IOperatorListGenerator _operatorListGenerator;
        private readonly IVesselClient _vesselClient;
        private readonly IRegistrationHistoryClient _registrationHistoryClient;
        private readonly ISightingClient _sightingClient;
        private readonly IShippingRecorderApplicationSettings _settings;
        private readonly ICacheWrapper _cache;
        private readonly ILogger<AddSightingWizard> _logger;

        public AddSightingWizard(
            ILocationClient locations,
            IVesselTypeListGenerator vesselTypes,
            ICountryListGenerator countries,
            IOperatorListGenerator operators,
            IVesselClient vessels,
            IRegistrationHistoryClient registrationHistory,
            ISightingClient sightings,
            IShippingRecorderApplicationSettings settings,
            ICacheWrapper cache,
            ILogger<AddSightingWizard> logger)
        {
            _locationClient = locations;
            _vesselTypeListGenerator = vesselTypes;
            _countryListGenerator = countries;
            _operatorListGenerator = operators;
            _vesselClient = vessels;
            _registrationHistoryClient = registrationHistory;
            _sightingClient = sightings;
            _settings = settings;
            _cache = cache;
            _logger = logger;
        }
        
        /// <summary>
        /// Return the available locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
            => await _locationClient.ListAsync(1, int.MaxValue);

        /// <summary>
        /// Retrieve or construct the sighting details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SightingDetailsViewModel> GetSightingDetailsModelAsync(string userName, long? sightingId)
        {
            _logger.LogDebug($"Resolving sighting details model for sighting ID {sightingId} for user {userName}");

            // Retrieve the model from the cache
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            if ((model == null) || (model.SightingId != sightingId))
            {
                // Not cached or the ID has changed, so create a new one and set the "last sighting added" message
                Sighting lastAdded = GetLastSightingAdded(userName);
                ClearCachedLastSightingAdded(userName);

                // If an existing sighting is specified, construct the model using its details
                if (sightingId != null)
                {
                    Sighting sighting = await _sightingClient.GetAsync(sightingId ?? 0);
                    model = new SightingDetailsViewModel
                    {
                        SightingId = sightingId,
                        LastSightingAdded = lastAdded,
                        Date = sighting.Date,
                        LocationId = sighting.LocationId,
                        IMO = sighting.Vessel.IMO
                    };
                }
                else
                {
                    _logger.LogDebug($"Creating new sighting details model");
                    model = new SightingDetailsViewModel
                    {
                        LastSightingAdded = lastAdded,
                        Date = GetDefaultDate(userName)
                    };
                }
            }

            // Set the available locations
            List<Location> locations = await GetLocationsAsync();
            model.SetLocations(locations);

            _logger.LogDebug($"Resolved sighting details model: {model}");

            return model;
        }

        /// <summary>
        /// Retrieve or constuct the vessel details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<VesselDetailsViewModel> GetVesselDetailsModelAsync(string userName)
        {
            _logger.LogDebug($"Resolving vessel details model for user {userName}");

            // Retrieve the model from the cache
            string key = GetCacheKey(VesselDetailsKeyPrefix, userName);
            VesselDetailsViewModel model = _cache.Get<VesselDetailsViewModel>(key);
            if (model == null)
            {
                _logger.LogDebug($"Creating new vessel details model");

                // Not cached, so create a new one, using the cached sighting details model to supply the vessel IMO
                key = GetCacheKey(SightingDetailsKeyPrefix, userName);
                SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);
                model = new VesselDetailsViewModel();
                model.Vessel.IMO = sighting.IMO;

                // Load vessel types, countries and operators
                model.VesselTypes = await _vesselTypeListGenerator.Create();
                model.Countries = await _countryListGenerator.Create();
                model.Operators = await _operatorListGenerator.Create();
            }

            // See if this is an existing vessel
            _logger.LogDebug($"Retrieving vessel with IMO {model.Vessel.IMO}");
            Vessel vessel = await _vesselClient.GetAsync(model.Vessel.IMO);
            _logger.LogDebug($"Retrieved vessel: {vessel}");

            if (vessel != null)
            {
                _logger.LogDebug($"Adding existing vessel details to model: {vessel}");

                // Existing vessel - get it's active registration details
                var registration = await _registrationHistoryClient.GetActiveRegistrationForVesselAsync(vessel.Id);

                // Add the vessel and registration to the model
                model.Vessel = vessel;
                model.Registration = registration ?? new() { Date = DateTime.Today };
                model.Editable = false;

                // Retrieve the most recent sighting of this vessel
                // TODO: Sighting search client
                // model.MostRecentSighting = await _sightingsSearch.GetMostRecentAircraftSighting(vessel.Registration);
            }

            _logger.LogDebug($"Resolved vessel details model: {model}");
            return model;
        }

        /// <summary>
        /// Retrieve the last sighting added
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Sighting GetLastSightingAdded(string userName)
        {
            string key = GetCacheKey(LastSightingAddedKeyPrefix, userName);
            return _cache.Get<Sighting>(key);
        }

        /// <summary>
        /// Cache the sighting details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheSightingDetailsModel(SightingDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            _cache.Set<SightingDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the vessel details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheVesselDetailsModel(VesselDetailsViewModel model, string userName)
        {
            string key = GetCacheKey(VesselDetailsKeyPrefix, userName);
            _cache.Set<VesselDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Clear the cached sighting details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedSightingDetailsModel(string userName)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached vessel details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedVesselDetailsModel(string userName)
        {
            string key = GetCacheKey(VesselDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the last sighting added from the cache
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedLastSightingAdded(string userName)
        {
            string key = GetCacheKey(LastSightingAddedKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Reset the wizard by clearing the cached details models
        /// </summary>
        /// <param name="userName"></param>
        public void Reset(string userName)
        {
            ClearCachedSightingDetailsModel(userName);
        }

        /// <summary>
        /// The sighting date is cached between entries to speed up multiple
        /// entries on the same date. If it's not been cached yet, it defaults
        /// to today
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private DateTime GetDefaultDate(string userName)
        {
            string key = GetCacheKey(DefaultDateKeyPrefix, userName);
            DateTime? defaultDate = _cache.Get<DateTime?>(key);
            return defaultDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }

        /// <summary>
        /// Construct a key for caching data
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static string GetCacheKey(string prefix, string userName)
        {
            string key = $"{prefix}.{userName}";
            return key;
        }
    }
}
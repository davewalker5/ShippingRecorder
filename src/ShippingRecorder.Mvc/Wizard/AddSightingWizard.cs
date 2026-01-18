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
        private readonly IVoyageClient _voyageClient;
        private readonly IRegistrationHistoryClient _registrationHistoryClient;
        private readonly ISightingClient _sightingClient;
        private readonly IShippingRecorderApplicationSettings _settings;
        private readonly IShippingRecorderCache _cache;
        private readonly ILogger<AddSightingWizard> _logger;

        public AddSightingWizard(
            ILocationClient locations,
            IVesselTypeListGenerator vesselTypes,
            ICountryListGenerator countries,
            IOperatorListGenerator operators,
            IVesselClient vessels,
            IVoyageClient voyages,
            IRegistrationHistoryClient registrationHistory,
            ISightingClient sightings,
            IShippingRecorderApplicationSettings settings,
            IShippingRecorderCache cache,
            ILogger<AddSightingWizard> logger)
        {
            _locationClient = locations;
            _vesselTypeListGenerator = vesselTypes;
            _countryListGenerator = countries;
            _operatorListGenerator = operators;
            _vesselClient = vessels;
            _voyageClient = voyages;
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
        /// Return the available voyages
        /// </summary>
        /// <returns></returns>
        public async Task<List<Voyage>> GetVoyagesAsync()
            => await _voyageClient.ListAsync(0, 1, int.MaxValue);

        /// <summary>
        /// Get the sighting ID for the current sighting being edited, which will
        /// be null for new sightings
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public long? GetCurrentSightingId(string userName)
        {
            _logger.LogDebug($"Retrieving current sighting ID for user {userName}");
            string key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            _logger.LogDebug($"Current sighting ID for user {userName} = {model?.SightingId}");
            return model?.SightingId;
        }

        /// <summary>
        /// Retrieve or construct the sighting details model
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SightingDetailsViewModel> GetSightingDetailsModelAsync(string userName, long? sightingId)
        {
            _logger.LogDebug($"Resolving sighting details model for sighting ID {sightingId} for user {userName}");

            // Retrieve the model from the cache
            string key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel model = _cache.Get<SightingDetailsViewModel>(key);
            if ((model == null) || (model.SightingId != sightingId))
            {
                _logger.LogDebug($"Loading existing sighting with ID {sightingId}");

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
                        VoyageId = sighting.VoyageId ?? 0,
                        VoyageName = sighting.Voyage?.ToString() ?? "",
                        Identifier = sighting.Vessel.Identifier
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

            // Set the available voyages
            List<Voyage> voyages = await GetVoyagesAsync();
            model.SetVoyages(voyages);

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
            string key = _cache.GetCacheKey(VesselDetailsKeyPrefix, userName);
            VesselDetailsViewModel model = _cache.Get<VesselDetailsViewModel>(key);
            if (model == null)
            {
                _logger.LogDebug($"Creating new vessel details model");

                // Not cached, so create a new one, using the cached sighting details model to supply the vessel identifier
                key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
                SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);
                model = new VesselDetailsViewModel();
                model.Vessel.Identifier = sighting.Identifier;

                // Load vessel types, countries and operators
                model.VesselTypes = await _vesselTypeListGenerator.Create();
                model.Countries = await _countryListGenerator.Create();
                model.Operators = await _operatorListGenerator.Create();
            }

            // See if this is an existing vessel
            _logger.LogDebug($"Retrieving vessel with  {model.Vessel.Identifier}");
            Vessel vessel = await _vesselClient.GetAsync(model.Vessel.Identifier);
            _logger.LogDebug($"Retrieved vessel: {vessel}");

            if (vessel != null)
            {
                _logger.LogDebug($"Adding existing vessel details to model: {vessel}");

                // Add the vessel and registration to the model
                model.Vessel = vessel;
                model.Registration = vessel.ActiveRegistrationHistory ?? new() { Date = DateTime.Today };
                model.Editable = false;

                // Retrieve the most recent sighting of this vessel
                model.MostRecentSighting = await _sightingClient.GetMostRecentVesselSightingAsync(vessel.Identifier);
            }

            _logger.LogDebug($"Resolved vessel details model: {model}");
            return model;
        }

        /// <summary>
        /// Construct the model to confirm sighting details
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ConfirmDetailsViewModel> GetConfirmDetailsModelAsync(string userName)
        {
            _logger.LogDebug($"Creating confirm details model for user {userName}");

            // Retrieve the sighting details model from the cache
            string key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel sighting = _cache.Get<SightingDetailsViewModel>(key);

            _logger.LogDebug($"Retrieved sighting details: {sighting}");

            // Retrieve the vessel details model from the cache
            key = _cache.GetCacheKey(VesselDetailsKeyPrefix, userName);
            VesselDetailsViewModel vesselDetails = _cache.Get<VesselDetailsViewModel>(key);

            _logger.LogDebug($"Retrieved vessel details: {vesselDetails}");

            // Use them to construct the confirm details model
            ConfirmDetailsViewModel model = new ConfirmDetailsViewModel()
            {
                Editable = false,
                Date = sighting.Date ?? DateTime.Today,
                Vessel = vesselDetails.Vessel,
                Registration = vesselDetails.Registration,
                VesselTypes = await _vesselTypeListGenerator.Create(),
                Countries = await _countryListGenerator.Create(),
                Operators = await _operatorListGenerator.Create()
            };

            // Get the voyage, if specified
            if (sighting.VoyageId > 0)
            {
                Voyage voyage = await _voyageClient.GetAsync(sighting.VoyageId);
                model.Voyage = voyage.Number;
            }

            // For the location, if we have a new location specified then use that as the
            // location. Otherwise, look up the location by its ID
            if (sighting.LocationId == 0)
            {
                model.Location = sighting.NewLocation;
            }
            else
            {
                Location location = await _locationClient.GetAsync(sighting.LocationId);
                model.Location = location.Name;
            }

            _logger.LogDebug($"Created confirm details model: {model}");
            return model;
        }

        /// <summary>
        /// Create a new sighting
        /// </summary>
        /// <param name="userName"></param>
        public async Task<Sighting> CreateSighting(string userName)
        {
            Sighting sighting = null;

            _logger.LogDebug($"Creating new sighting for user {userName}");

            // Clear the last sighting added message
            ClearCachedLastSightingAdded(userName);

            // Retrieve the sighting details model from the cache
            string key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
            SightingDetailsViewModel details = _cache.Get<SightingDetailsViewModel>(key);

            if (details != null)
            {
                // Create the vessel and registration history
                Vessel vessel = await RetrieveOrCreateVessel(userName);

                // Create the location, if required
                if (details.LocationId == 0)
                {
                    Location location = await _locationClient.AddAsync(details.NewLocation);
                    details.LocationId = location.Id;
                }

                // Determine the ID for the selected voyage
                long? voyageId = details.VoyageId > 0 ? details.VoyageId : null;

                // If an existing sighting is being edited, then update it. Otherwise, create a new one
                if (details.SightingId != null)
                {
                    _logger.LogDebug($"Updating existing sighting with ID {details.SightingId} for user {userName}");

                    sighting = await _sightingClient.UpdateAsync(
                        details.SightingId ?? 0,
                        details.LocationId,
                        voyageId,
                        vessel.Id,
                        details.Date ?? DateTime.Now,
                        details.IsMyVoyage);
                }
                else
                {
                    _logger.LogDebug($"Creating new sighting for user {userName}");

                    sighting = await _sightingClient.AddAsync(
                        details.LocationId,
                        voyageId,
                        vessel.Id,
                        details.Date ?? DateTime.Now,
                        details.IsMyVoyage);
                }

                // Cache the sighting details and other properties that are cached to improve data entry speed
                key = _cache.GetCacheKey(LastSightingAddedKeyPrefix, userName);
                _cache.Set<Sighting>(key, sighting, _settings.CacheLifetimeSeconds);

                key = _cache.GetCacheKey(DefaultDateKeyPrefix, userName);
                _cache.Set<DateTime>(key, sighting.Date, _settings.CacheLifetimeSeconds);
            }

            // Clear the cached data
            Reset(userName);

            _logger.LogDebug($"Created sighting: {sighting}");

            return sighting;
        }

        /// <summary>
        /// Either retrieve an existing vessel or create a new one
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<Vessel> RetrieveOrCreateVessel(string userName)
        {
            Vessel vessel = null;

            _logger.LogDebug($"Creating or retrieving vessel for {userName}");

            // Retrieve the aircraft details from the cache
            string key = _cache.GetCacheKey(VesselDetailsKeyPrefix, userName);
            VesselDetailsViewModel details = _cache.Get<VesselDetailsViewModel>(key);
            if (details != null)
            {
                _logger.LogDebug($"Retrieved aircraft details model: {details}");

                // If this is a sighting for an existing aircraft, just return it.
                // Otherwise, we need to create a new aircraft
                if (details.Vessel.Id > 0)
                {
                    _logger.LogDebug($"Retrieving vessel with Id: {details.Vessel.Id}");
                    vessel = await _vesselClient.GetAsync(details.Vessel.Id);
                }
                else
                {
                    _logger.LogDebug($"Creating new vessel");

                    // Create the vessel
                    vessel = await _vesselClient.AddAsync(
                        details.Vessel.Identifier,
                        details.Vessel.IsIMO,
                        details.Vessel.Built,
                        details.Vessel.Draught,
                        details.Vessel.Length,
                        details.Vessel.Beam);

                    // Add the registration details
                    var registration = await _registrationHistoryClient.AddAsync(
                        vessel.Id,
                        details.Registration.VesselTypeId,
                        details.Registration.FlagId,
                        details.Registration.OperatorId,
                        details.Registration.Date,
                        details.Registration.Name,
                        details.Registration.Callsign,
                        details.Registration.MMSI,
                        details.Registration.Tonnage,
                        details.Registration.Passengers,
                        details.Registration.Crew,
                        details.Registration.Decks,
                        details.Registration.Cabins);

                    // Attach the registration history to the vessel
                    vessel.RegistrationHistory.Add(registration);
                }
            }

            _logger.LogDebug($"Retrieved or created vessel: {vessel}");
            return vessel;
        }

        /// <summary>
        /// Retrieve the last sighting added
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Sighting GetLastSightingAdded(string userName)
        {
            string key = _cache.GetCacheKey(LastSightingAddedKeyPrefix, userName);
            return _cache.Get<Sighting>(key);
        }

        /// <summary>
        /// Cache the sighting details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheSightingDetailsModel(SightingDetailsViewModel model, string userName)
        {
            string key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
            _cache.Set<SightingDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Cache the vessel details view model
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="model"></param>
        public void CacheVesselDetailsModel(VesselDetailsViewModel model, string userName)
        {
            string key = _cache.GetCacheKey(VesselDetailsKeyPrefix, userName);
            _cache.Set<VesselDetailsViewModel>(key, model, _settings.CacheLifetimeSeconds);
        }

        /// <summary>
        /// Clear the cached sighting details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedSightingDetailsModel(string userName)
        {
            string key = _cache.GetCacheKey(SightingDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the cached vessel details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedVesselDetailsModel(string userName)
        {
            string key = _cache.GetCacheKey(VesselDetailsKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Clear the last sighting added from the cache
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedLastSightingAdded(string userName)
        {
            string key = _cache.GetCacheKey(LastSightingAddedKeyPrefix, userName);
            _cache.Remove(key);
        }

        /// <summary>
        /// Reset the wizard by clearing the cached details models
        /// </summary>
        /// <param name="userName"></param>
        public void Reset(string userName)
        {
            ClearCachedSightingDetailsModel(userName);
            ClearCachedVesselDetailsModel(userName);
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
            string key = _cache.GetCacheKey(DefaultDateKeyPrefix, userName);
            DateTime? defaultDate = _cache.Get<DateTime?>(key);
            return defaultDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        }
    }
}
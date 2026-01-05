using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Mvc.Models;

namespace ShippingRecorder.Mvc.Wizard
{
    public class AddSightingWizard
    {
        private const string SightingDetailsKeyPrefix = "Wizard.SightingDetails";
        private const string LastSightingAddedKeyPrefix = "Wizard.LastSightingAdded";
        private const string DefaultDateKeyPrefix = "Wizard.DefaultDate";

        private readonly ILocationClient _locations;
        private readonly ISightingClient _sightings;
        private readonly IShippingRecorderApplicationSettings _settings;
        private readonly ICacheWrapper _cache;
        private readonly ILogger<AddSightingWizard> _logger;

        public AddSightingWizard(
            ILocationClient locations,
            ISightingClient sightings,
            IShippingRecorderApplicationSettings settings,
            ICacheWrapper cache,
            ILogger<AddSightingWizard> logger)
        {
            _locations = locations;
            _sightings = sightings;
            _settings = settings;
            _cache = cache;
            _logger = logger;
        }
        
        /// <summary>
        /// Return the available locations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Location>> GetLocationsAsync()
            => await _locations.ListAsync(1, int.MaxValue);

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
                    Sighting sighting = await _sightings.GetAsync(sightingId ?? 0);
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
        /// Clear the cached sighting details model
        /// </summary>
        /// <param name="userName"></param>
        public void ClearCachedSightingDetailsModel(string userName)
        {
            string key = GetCacheKey(SightingDetailsKeyPrefix, userName);
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
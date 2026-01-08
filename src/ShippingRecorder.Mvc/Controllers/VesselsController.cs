using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Mvc.Entities;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class VesselsController : ShippingRecorderControllerBase
    {
        private readonly IVesselClient _vesselClient;
        private readonly IRegistrationHistoryClient _registrationClient;
        private readonly ICountryListGenerator _countryListGenerator;
        private readonly IOperatorListGenerator _operatorListGenerator;
        private readonly IVesselTypeListGenerator _vesselTypesListGenerator;
        private readonly IShippingRecorderApplicationSettings _settings;

        public VesselsController(
            IVesselClient vesselClient,
            IRegistrationHistoryClient registrationClient,
            ICountryListGenerator countryListGenertor,
            IOperatorListGenerator operatorListGenerator,
            IVesselTypeListGenerator vesselTypesListGenertor,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<VesselsController> logger) : base (renderer, logger)
        {
            _vesselClient = vesselClient;
            _registrationClient = registrationClient;
            _countryListGenerator = countryListGenertor;
            _operatorListGenerator = operatorListGenerator;
            _vesselTypesListGenerator = vesselTypesListGenertor;
            _settings = settings;
        }

        /// <summary>
        /// Serve the vessel list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the list of current vessels
            List<Vessel> vessels = await _vesselClient.ListAsync(1, _settings.SearchPageSize) ?? [];
            var plural = vessels.Count == 1 ? "" : "s";
            _logger.LogDebug($"{vessels.Count} vessel{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new VesselListViewModel();
            model.SetVessels(vessels, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VesselListViewModel model)
        {
            if (ModelState.IsValid)
            {
                int page = model.PageNumber;
                switch (model.Action)
                {
                    case ControllerActions.ActionPreviousPage:
                        page -= 1;
                        break;
                    case ControllerActions.ActionNextPage:
                        page += 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                // Retrieve the matching airport records
                var vessels = await _vesselClient.ListAsync(page, _settings.SearchPageSize);
                model.SetVessels(vessels, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new vessel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View(new AddVesselViewModel()
            {
                Countries = await _countryListGenerator.Create(),
                Operators = await _operatorListGenerator.Create(),
                VesselTypes = await _vesselTypesListGenerator.Create()
            });
        }

        /// <summary>
        /// Handle POST events to save new vessels
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddVesselViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Adding vessel: " +
                    $"IMO = {model.Vessel.IMO}, " +
                    $"Built = {model.Vessel.Built}, " +
                    $"Draught = {model.Vessel.Draught}, " +
                    $"Length = {model.Vessel.Length}, " +
                    $"Beam = {model.Vessel.Beam}, " +
                    $"Vessel Type ID = {model.Registration.VesselTypeId}, " +
                    $"Flag ID = {model.Registration.FlagId}, " +
                    $"Operator ID = {model.Registration.OperatorId}, " +
                    $"Date = {model.Registration.Date}, " +
                    $"Name = {model.Registration.Name}, " +
                    $"Callsign = {model.Registration.Callsign}, " +
                    $"MMSI = {model.Registration.MMSI}, " +
                    $"Tonnage = {model.Registration.Tonnage}, " +
                    $"Passengers = {model.Registration.Passengers}, " +
                    $"Crew = {model.Registration.Crew}, " +
                    $"Decks = {model.Registration.Decks}, " +
                    $"Cabins = {model.Registration.Cabins}");

                Vessel vessel = await _vesselClient.AddAsync(
                    model.Vessel.IMO,
                    model.Vessel.Built,
                    model.Vessel.Draught,
                    model.Vessel.Length,
                    model.Vessel.Beam);

                await _registrationClient.AddAsync(
                    vessel.Id,
                    model.Registration.VesselTypeId,
                    model.Registration.FlagId,
                    model.Registration.OperatorId,
                    model.Registration.Date,
                    model.Registration.Name,
                    model.Registration.Callsign,
                    model.Registration.MMSI,
                    model.Registration.Tonnage,
                    model.Registration.Passengers,
                    model.Registration.Crew,
                    model.Registration.Decks,
                    model.Registration.Cabins);
    
                ModelState.Clear();
                model.Clear();
                model.Message = $"Vessel '{vessel.IMO}' added successfully";
            }
            else
            {
                LogModelState();
            }

            // Load the drop-down content
            model.Countries = await _countryListGenerator.Create();
            model.Operators = await _operatorListGenerator.Create();
            model.VesselTypes = await _vesselTypesListGenerator.Create();

            return View(model);
        }

        /// <summary>
        /// Serve the vessel editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Vessel vessel = await _vesselClient.GetAsync(id);
            return View(new VesselModel
            {
                Vessel = vessel,
                Registration = vessel.ActiveRegistrationHistory ?? new() { Date = DateTime.Now },
                Countries = await _countryListGenerator.Create(),
                Operators = await _operatorListGenerator.Create(),
                VesselTypes = await _vesselTypesListGenerator.Create()
            });
        }

        /// <summary>
        /// Handle POST events to save updated vessels
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VesselModel model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Updating vessel: ID = {model.Vessel.Id}, " +
                    $"IMO = {model.Vessel.IMO}, " +
                    $"Built = {model.Vessel.Built}, " +
                    $"Draught = {model.Vessel.Draught}, " +
                    $"Length = {model.Vessel.Length}, " +
                    $"Beam = {model.Vessel.Beam}, " +
                    $"Vessel Type ID = {model.Registration.VesselTypeId}, " +
                    $"Flag ID = {model.Registration.FlagId}, " +
                    $"Operator ID = {model.Registration.OperatorId}, " +
                    $"Date = {model.Registration.Date}, " +
                    $"Name = {model.Registration.Name}, " +
                    $"Callsign = {model.Registration.Callsign}, " +
                    $"MMSI = {model.Registration.MMSI}, " +
                    $"Tonnage = {model.Registration.Tonnage}, " +
                    $"Passengers = {model.Registration.Passengers}, " +
                    $"Crew = {model.Registration.Crew}, " +
                    $"Decks = {model.Registration.Decks}, " +
                    $"Cabins = {model.Registration.Cabins}");

                var vessel = await _vesselClient.UpdateAsync(
                    model.Vessel.Id,
                    model.Vessel.IMO,
                    model.Vessel.Built,
                    model.Vessel.Draught,
                    model.Vessel.Length,
                    model.Vessel.Beam);

                // Load the original registration history for the vessel and see if there are any changes
                RegistrationHistory registration = vessel.ActiveRegistrationHistory;
                if (model.Registration != registration)
                {
                    await _registrationClient.AddAsync(
                        model.Vessel.Id,
                        model.Registration.VesselTypeId,
                        model.Registration.FlagId,
                        model.Registration.OperatorId,
                        model.Registration.Date,
                        model.Registration.Name,
                        model.Registration.Callsign,
                        model.Registration.MMSI,
                        model.Registration.Tonnage,
                        model.Registration.Passengers,
                        model.Registration.Crew,
                        model.Registration.Decks,
                        model.Registration.Cabins);
                }

                result = RedirectToAction("Index");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            // Load the drop-down content
            model.Countries = await _countryListGenerator.Create();
            model.Operators = await _operatorListGenerator.Create();
            model.VesselTypes = await _vesselTypesListGenerator.Create();

            return result;
        }

        /// <summary>
        /// Show the modal dialog containing the vessel details for the specified vessel
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ShowVesselDetails(long identifier)
        {
            var vessel = await _vesselClient.GetAsync(identifier);
            var model = new VesselDetailsViewModel()
            {
                Countries = await _countryListGenerator.Create(),
                Operators = await _operatorListGenerator.Create(),
                VesselTypes = await _vesselTypesListGenerator.Create(),
                Editable = false,
                Vessel = vessel,
                Registration = vessel.ActiveRegistrationHistory ?? new() { Date = DateTime.Today }
            };

            var title = $"Vessel Details for IMO {vessel.IMO}";
            return await LoadModalContent("_VesselDetails", model, title);
        }
    }
}

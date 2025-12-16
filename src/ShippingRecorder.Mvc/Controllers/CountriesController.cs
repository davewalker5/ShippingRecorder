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
    public class CountriesController : ShippingRecorderControllerBase
    {
        private readonly ICountryClient _client;
        private readonly IShippingRecorderApplicationSettings _settings;

        public CountriesController(
            ICountryClient client,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<CountriesController> logger) : base (renderer, logger)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the country list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var countries = await _client.ListAsync(1, _settings.SearchPageSize);
            var model = new CountryListViewModel();
            model.SetCountries(countries, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CountryListViewModel model)
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

                // Retrieve the matching country records
                var countries = await _client.ListAsync(page, _settings.SearchPageSize);
                model.SetCountries(countries, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new country
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddCountryViewModel());
        }

        /// <summary>
        /// Handle POST events to save new countries
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddCountryViewModel model)
        {
            if (ModelState.IsValid)
            {
                Country country = await _client.AddAsync(model.Code, model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Country '{country.Name}' added successfully";
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the country editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Country model = await _client.GetAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated countries
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                await _client.UpdateAsync(model.Id, model.Code, model.Name);
                result = RedirectToAction("Index");
            }
            else
            {
                LogModelState();
                result = View(model);
            }

            return result;
        }
    }
}

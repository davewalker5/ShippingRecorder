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
    public class OperatorsController : ShippingRecorderControllerBase
    {
        private readonly IOperatorClient _client;
        private readonly IShippingRecorderApplicationSettings _settings;

        public OperatorsController(
            IOperatorClient client,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<OperatorsController> logger) : base (renderer, logger)
        {
            _client = client;
            _settings = settings;
        }

        /// <summary>
        /// Serve the operators list page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get the list of current operators
            List<Operator> operators = await _client.ListAsync(1, _settings.SearchPageSize) ?? [];
            var plural = operators.Count == 1 ? "" : "s";
            _logger.LogDebug($"{operators.Count} operator{plural} loaded via the service");

            // Construct the view model and serve the page
            var model = new OperatorListViewModel();
            model.SetOperators(operators, 1, _settings.SearchPageSize);
            return View(model);
        }

        /// <summary>
        /// Handle POST events for page navigation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OperatorListViewModel model)
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

                // Retrieve the matching operator records
                var operators = await _client.ListAsync(page, _settings.SearchPageSize);
                model.SetOperators(operators, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the page to add a new operator
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Add()
        {
            return View(new AddOperatorViewModel());
        }

        /// <summary>
        /// Handle POST events to save new operators
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddOperatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Adding operator: Name = {model.Name}");
                Operator op = await _client.AddAsync(model.Name);
                ModelState.Clear();
                model.Clear();
                model.Message = $"Operator '{op.Name}' added successfully";
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }

        /// <summary>
        /// Serve the operator editing page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Operator model = await _client.GetAsync(id);
            return View(model);
        }

        /// <summary>
        /// Handle POST events to save updated operators
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Operator model)
        {
            IActionResult result;

            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Updating operator: ID = {model.Id}, Name = {model.Name}");
                await _client.UpdateAsync(model.Id, model.Name);
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

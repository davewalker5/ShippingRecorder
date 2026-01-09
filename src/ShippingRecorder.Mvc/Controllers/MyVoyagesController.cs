using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Reporting;
using ShippingRecorder.Mvc.Entities;
using ShippingRecorder.Mvc.Interfaces;
using ShippingRecorder.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Mvc.Controllers
{
    [Authorize]
    public class MyVoyagesController : ShippingRecorderControllerBase
    {
        private readonly IReportsClient _reportsClient;
        private readonly IShippingRecorderApplicationSettings _settings;

        public MyVoyagesController(
            IReportsClient iReportsClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<MyVoyagesController> logger) : base (renderer, logger)
        {
            _reportsClient = iReportsClient;
            _settings = settings;
        }

        /// <summary>
        /// Serve the empty report page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            MyVoyagesViewModel model = new MyVoyagesViewModel
            {
                PageNumber = 1
            };
            return View(model);
        }

        /// <summary>
        /// Respond to a POST event triggering the report generation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(MyVoyagesViewModel model)
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
                    case ControllerActions.ActionSearch:
                        page = 1;
                        break;
                    default:
                        break;
                }

                // Need to clear model state here or the page number that was posted
                // is returned and page navigation doesn't work correctly. So, capture
                // and amend the page number, above, then apply it, below
                ModelState.Clear();

                List<MyVoyages> records = await _reportsClient.MyVoyagesAsync(model.From, model.To, page, _settings.SearchPageSize);
                model.SetRecords(records, page, _settings.SearchPageSize);
            }
            else
            {
                LogModelState();
            }

            return View(model);
        }
    }
}

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
    public class OperatorStatisticsController : ShippingRecorderControllerBase
    {
        private readonly IReportsClient _reportsClient;
        private readonly IShippingRecorderApplicationSettings _settings;

        public OperatorStatisticsController(
            IReportsClient iReportsClient,
            IShippingRecorderApplicationSettings settings,
            IPartialViewToStringRenderer renderer,
            ILogger<OperatorStatisticsController> logger) : base (renderer, logger)
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
            OperatorStatisticsViewModel model = new OperatorStatisticsViewModel
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
        public async Task<IActionResult> Index(OperatorStatisticsViewModel model)
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

                DateTime start = model.From ?? DateTime.MinValue;
                DateTime end = model.To ?? DateTime.MaxValue;

                List<OperatorStatistics> records = await _reportsClient.OperatorStatisticsAsync(start, end, page, _settings.SearchPageSize);
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

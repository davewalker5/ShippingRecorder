using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Entities.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using ShippingRecorder.Entities.Reporting;

namespace ShippingRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class ReportsController : Controller
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        private readonly ShippingRecorderFactory _factory;

        public ReportsController(ShippingRecorderFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Generate the job statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("jobs/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<JobStatus>>> GetJobsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.JobStatuses
                                        .ListAsync(x => (x.Start >= startDate) && ((x.End == null) || (x.End <= endDate)),
                                                   pageNumber,
                                                   pageSize)
                                        .ToListAsync();

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results;
        }

        /// <summary>
        /// Generate the location statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("locations/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<LocationStatistics>>> GetLocationStatisticsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.LocationStatistics.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the sightings by month statistics report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sightings/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<SightingsByMonth>>> GetSightingsByMonthAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.SightingsByMonth.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }

        /// <summary>
        /// Generate the "My Voyages" report
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("myvoyages/{start}/{end}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<MyVoyages>>> GetMyFlightsAsync(string start, string end, int pageNumber, int pageSize)
        {
            // Decode the start and end date and convert them to dates
            DateTime startDate = DateTime.ParseExact(HttpUtility.UrlDecode(start), DateTimeFormat, null);
            DateTime endDate = DateTime.ParseExact(HttpUtility.UrlDecode(end), DateTimeFormat, null);

            // Get the report content
            var results = await _factory.MyVoyages.GenerateReportAsync(startDate, endDate, pageNumber, pageSize);

            if (!results.Any())
            {
                return NoContent();
            }

            // Convert to a list and return the results
            return results.ToList();
        }
    }
}

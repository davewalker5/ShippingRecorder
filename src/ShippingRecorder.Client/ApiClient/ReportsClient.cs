using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Db;
using Microsoft.Extensions.Logging;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Reporting;
using System.Web;

namespace ShippingRecorder.Client.ApiClient
{
    public class ReportsClient : ShippingRecorderClientBase, IReportsClient
    {
        private readonly DateTime DefaultMinimumDate = new(1970, 1, 1, 0, 0, 0);

        public ReportsClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<ReportsClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Return the job status report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<JobStatus>> JobStatusAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<JobStatus>("JobStatus", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the location statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<LocationStatistics>> LocationStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<LocationStatistics>("LocationStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the sightings by month report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<SightingsByMonth>> SightingsByMonthAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<SightingsByMonth>("SightingsByMonth", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the "my voyages" report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<MyVoyages>> MyVoyagesAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<MyVoyages>("MyVoyages", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the operator statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<OperatorStatistics>> OperatorStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<OperatorStatistics>("OperatorStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the vessel type statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<VesselTypeStatistics>> VesselTypeStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<VesselTypeStatistics>("VesselTypeStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return the flag statistics report
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<List<FlagStatistics>> FlagStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
            => await DateBasedReportAsync<FlagStatistics>("FlagStatistics", from, to, pageNumber, pageSize);

        /// <summary>
        /// Return a date-based statistics report
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routeName"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<List<T>> DateBasedReportAsync<T>(string routeName, DateTime? from, DateTime? to, int pageNumber, int pageSize)
        {
            // Make sure the dates passed to the API aren't NULL
            var nonNullFrom = (from ?? DefaultMinimumDate).ToString(Settings.DateTimeFormat);
            var nonNullTo = (to ?? DateTime.Now).ToString(Settings.DateTimeFormat);

            // URL encode the dates
            string fromRouteSegment = HttpUtility.UrlEncode(nonNullFrom);
            string toRouteSegment = HttpUtility.UrlEncode(nonNullTo);

            Logger.LogDebug($"Date Time Formatter is {Settings.DateTimeFormat}");
            Logger.LogDebug($"{nonNullFrom} encoded as {fromRouteSegment}");
            Logger.LogDebug($"{nonNullTo} encoded as {toRouteSegment}");

            // Construct the route
            string route = @$"{Settings.ApiRoutes.First(r => r.Name == routeName).Route}/{fromRouteSegment}/{toRouteSegment}/{pageNumber}/{pageSize}";

            // Call the endpoint and decode the response
            string json = await SendDirectAsync(route, null, HttpMethod.Get);
            var records = Deserialize<List<T>>(json);

            return records;
        }
    }
}

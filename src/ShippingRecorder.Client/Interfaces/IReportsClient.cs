using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Reporting;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IReportsClient
    {
        Task<List<JobStatus>> JobStatusAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<LocationStatistics>> LocationStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<SightingsByMonth>> SightingsByMonthAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<MyVoyages>> MyVoyagesAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<OperatorStatistics>> OperatorStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<VesselTypeStatistics>> VesselTypeStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<FlagStatistics>> FlagStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.Client.Interfaces
{
    public interface IReportsClient
    {
        Task<List<JobStatus>> JobStatusAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}

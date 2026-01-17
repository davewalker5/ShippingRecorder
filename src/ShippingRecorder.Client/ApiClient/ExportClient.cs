using ShippingRecorder.Client.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ShippingRecorder.Entities.Db;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;

namespace ShippingRecorder.Client.ApiClient
{
    public class ExportClient : ShippingRecorderClientBase, IExportClient
    {
        private const string RouteKey = "Export";

        public ExportClient(
            IShippingRecorderHttpClient client,
            IShippingRecorderApplicationSettings settings,
            IAuthenticationTokenProvider tokenProvider,
            ICacheWrapper cache,
            ILogger<ExportClient> logger)
            : base(client, settings, tokenProvider, cache, logger)
        {
        }

        /// <summary>
        /// Request an export of allexports to a named file in the export allexport
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ExportAsync(string fileName)
        {
            dynamic data = new{ FileName = fileName };
            var json = Serialize(data);
            await SendIndirectAsync(RouteKey, json, HttpMethod.Post);
        }
    }
}

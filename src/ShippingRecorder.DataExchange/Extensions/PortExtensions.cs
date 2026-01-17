using System.Collections.Generic;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Extensions
{
    public static class PortExtensions
    {
        /// <summary>
        /// Return an exportable port from a port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static ExportablePort ToExportable(this Port port)
            => new()
            {
                Code = port.Code,
                Name = port.Name
            };

        /// <summary>
        /// Return a collection of exportable ports from a collection of ports
        /// </summary>
        /// <param name="ports"></param>
        /// <returns></returns>
        public static IEnumerable<ExportablePort> ToExportable(this IEnumerable<Port> ports)
        {
            var exportable = new List<ExportablePort>();

            foreach (var port in ports)
            {
                exportable.Add(port.ToExportable());
            }

            return exportable;
        }
    }
}

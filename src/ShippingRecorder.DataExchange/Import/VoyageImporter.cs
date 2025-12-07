using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class VoyageImporter : CsvImporter<ExportableVoyage>, IVoyageImporter
    {
        private List<Operator> _operators;

        public VoyageImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
        {

        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _operators = await _factory.Operators.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableVoyage Inflate(string record)
            => ExportableVoyage.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="voyage"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableVoyage voyage, int recordCount)
        {
            ValidateField<string>(x => CheckOperatorExists(x), voyage.Operator,  "Operator", recordCount);
            ValidateField<string>(x => CheckPortExists(x), voyage.Port,  "Port", recordCount);
            ValidateField<string>(x => Enum.TryParse<VoyageEventType>(x, out _), voyage.EventType, "EventType", recordCount);
            ValidateField<string>(x => CheckDateFormat(x), voyage.Date, "Date", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="vessel"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableVoyage voyage)
        {
            // Load/create the voyage
            var op = _operators.First(x => x.Name == voyage.Operator);
            var imported = await _factory.Voyages.GetAsync(x => (x.OperatorId == op.Id) && (x.Number == voyage.Number));
            imported ??= await _factory.Voyages.AddAsync(op.Id, voyage.Number);

            // Add the event
            var port = await _factory.Ports.GetAsync(x => x.Code == voyage.Port);
            var type = Enum.Parse<VoyageEventType>(voyage.EventType, true);
            var date = DateTime.ParseExact(voyage.Date, ExportableEntityBase.DateFormat, CultureInfo.InvariantCulture);
            _ = await _factory.VoyageEvents.AddAsync(imported.Id, port.Id, type, date);
        }

        /// <summary>
        /// Check an operator exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckOperatorExists(string name)
            => _operators.FirstOrDefault(x => x.Name == name) != null;

        /// <summary>
        /// Check a port exists
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private bool CheckPortExists(string code)
        {
            var port = Task.Run(() => _factory.Ports.GetAsync(x => x.Code == code)).Result;
            return port != null;
        }

        /// <summary>
        /// Check a date is correctly formatted
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool CheckDateFormat(string date)
            => DateTime.TryParseExact(
                    date,
                    ExportableEntityBase.DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime _);
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Attributes;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableVoyage : ExportableEntityBase
    {
        /// <summary>
        /// Vessel Identifier
        /// Operator
        /// Number
        /// Event Type
        /// Port UN/LOCODE
        /// Date
        /// </summary>
        public const string CsvRecordPattern = @"^""\d{7}"",(?:""(?!\s*"")[\s\S]*"",){3}""[A-Za-z]{2}[A-Za-z0-9]{3}"",""[0-9]{2}-[A-Za-z]{3}-[0-9]{4}"".?$";

        [Export("Identifier", 1)]
        public string Identifier { get; set; }

        [Export("Operator", 1)]
        public string Operator { get; set; }

        [Export("Number", 2)]
        public string Number { get; set; }

        [Export("Event Type", 3)]
        public string EventType { get; set; }

        [Export("Port", 4)]
        public string Port { get; set; }

        [Export("Date", 5)]
        public string Date { get; set; }

        public static ExportableVoyage FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableVoyage
            {
                Identifier = words[0].Replace("\"", "").Trim().CleanCode(),
                Operator = words[1].Replace("\"", "").Trim().TitleCase(),
                Number = words[2].Replace("\"", "").Trim().CleanCode(),
                EventType = words[3].Replace("\"", "").Trim(),
                Port = words[4].Replace("\"", "").Trim().CleanCode(),
                Date = words[5].Replace("\"", "").Trim()
            };
        }
    }
}

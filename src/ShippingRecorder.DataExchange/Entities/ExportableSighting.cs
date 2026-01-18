using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Attributes;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableSighting : ExportableEntityBase
    {
        public const string CsvRecordPattern = @"^""[0-9]+-[A-Za-z]+-[0-9]+"",""(?!\s*"")[\s\S]*"",""\d{7}"","".*"",""True|False"".?$";

        [Export("Date", 1)]
        public DateTime Date { get; set; }

        [Export("Location", 2)]
        public string Location { get; set; }

        [Export("Identifier", 3)]
        public string Identifier { get; set; }

        [Export("Voyage", 4)]
        public string VoyageNumber { get; set; }

        [Export("My Voyage", 5)]
        public bool IsMyVoyage { get; set; }

        public static ExportableSighting FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableSighting
            {
                Date = DateTime.ParseExact(words[0].Replace("\"", "").Trim(), DateFormat, CultureInfo.CurrentCulture),
                Location = words[1].Replace("\"", "").Trim(),
                Identifier = words[2].Replace("\"", "").Trim().CleanCode(),
                VoyageNumber = words[3].Replace("\"", "").Trim().Clean(),
                IsMyVoyage = words[4].Replace("\"", "").Trim().Equals("True", StringComparison.OrdinalIgnoreCase)
            };
        }
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Attributes;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableVessel : ExportableEntityBase
    {
        /// <summary>
        /// Identifier
        /// Identifier is an IMO
        /// Year built (may be blank)
        /// Maximum draught (may be blank)
        /// Length (may be blank)
        /// Beam (may be blank)
        /// Tonnage (may be blank)
        /// Passengers (may be blank)
        /// Crew (may be blank)
        /// Decks (may be blank)
        /// Cabins (may be blank)
        /// Vessel Name
        /// Callsign
        /// MMSI
        /// Vessel Type
        /// Flag (Country Code)
        /// Operator Name
        /// </summary>
        public const string CsvRecordPattern = @"^""\d{7}"",""True|False"",""\d*"",""[0-9.]*"",""\d*"",""\d*"",""\d*"",""\d*"",""\d*"",""\d*"",""\d*"",""[^""]+"",""[^""\s]+"",""\d{9}"",""[^""]+"",""[A-Za-z0-9]{2}"",""[^""]+"".?$";

        [Export("Identifier", 1)]
        public string Identifier { get; set; }

        [Export("Is IMO", 2)]
        public bool IsIMO { get; set; }

        [Export("Year Built", 3)]
        public int? Built { get; set; }

        [Export("Draught", 4)]
        public decimal? Draught { get; set; }

        [Export("Length", 5)]
        public int? Length { get; set; }

        [Export("Beam", 6)]
        public int? Beam { get; set; }

        [Export("Tonnage", 7)]
        public int? Tonnage { get; set; }

        [Export("Passengers", 8)]
        public int? Passengers { get; set; }

        [Export("Crew", 9)]
        public int? Crew { get; set; }

        [Export("Decks", 10)]
        public int? Decks { get; set; }

        [Export("Cabins", 11)]
        public int? Cabins { get; set; }

        [Export("Name", 12)]
        public string Name { get; set; }

        [Export("Callsign", 13)]
        public string Callsign { get; set; }

        [Export("MMSI", 14)]
        public string MMSI { get; set; }

        [Export("Type", 15)]
        public string VesselType { get; set; }

        [Export("Flag", 16)]
        public string Flag { get; set; }

        [Export("Operator", 17)]
        public string Operator { get; set; }

        public static ExportableVessel FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableVessel
            {
                Identifier = words[0].Replace("\"", "").Trim().CleanCode(),
                IsIMO = words[1].Replace("\"", "").Trim().Equals("True", StringComparison.OrdinalIgnoreCase),
                Built = ExtractInteger(words[2].Replace("\"", "").Trim()),
                Draught = ExtractDecimal(words[3].Replace("\"", "").Trim()),
                Length = ExtractInteger(words[4].Replace("\"", "").Trim()),
                Beam = ExtractInteger(words[5].Replace("\"", "").Trim()),
                Tonnage = ExtractInteger(words[6].Replace("\"", "").Trim()),
                Passengers = ExtractInteger(words[7].Replace("\"", "").Trim()),
                Crew = ExtractInteger(words[8].Replace("\"", "").Trim()),
                Decks = ExtractInteger(words[9].Replace("\"", "").Trim()),
                Cabins = ExtractInteger(words[10].Replace("\"", "").Trim()),
                Name = words[11].Replace("\"", "").Trim(),
                Callsign = words[12].Replace("\"", "").Trim().CleanCode(),
                MMSI = words[13].Replace("\"", "").Trim().CleanCode(),
                VesselType = words[14].Replace("\"", "").Trim().TitleCase(),
                Flag = words[15].Replace("\"", "").Trim().CleanCode(),
                Operator = words[16].Replace("\"", "").Trim().TitleCase()
            };
        }

        private static decimal? ExtractDecimal(string word)
            => string.IsNullOrEmpty(word) ? null : decimal.Parse(word);

        private static int? ExtractInteger(string word)
            => string.IsNullOrEmpty(word) ? null : int.Parse(word);
    }
}

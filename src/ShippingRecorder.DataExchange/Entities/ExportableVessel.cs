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
        /// IMO
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
        // public const string CsvRecordPattern = @"^""[0-9]{7}"",""(?:\d+)?"",""(?:\d+(?:\.\d+)?)?"",""(?:\d+)?"",""(?:\d+)?"",""(?:\d+)?"",""(?:\d+)?"",""(?:\d+)?"",""(?:\d+)?"",""(?:\d+)?"",""(?!\s*"")[\s\S]*"",""[A-Za-z0-9]+"",""[0-9]{9}"",""(?!\s*"")[\s\S]*"",""[A-Za-z]{2}"",""(?!\s*"")[\s\S]*"".?$";
        public const string CsvRecordPattern = @"^""\d{7}"",""\d*"",""[0-9.]*"",""\d*"",""\d*"",""\d*"",""\d*"",""\d*"",""\d*"",""\d*"",""[^""]+"",""[^""\s]+"",""\d{9}"",""[^""]+"",""[A-Za-z0-9]{2}"",""[^""]+"".?$";

        [Export("IMO", 1)]
        public string IMO { get; set; }

        [Export("Year Built", 2)]
        public int? Built { get; set; }

        [Export("Draught", 3)]
        public decimal? Draught { get; set; }

        [Export("Length", 4)]
        public int? Length { get; set; }

        [Export("Beam", 5)]
        public int? Beam { get; set; }

        [Export("Tonnage", 6)]
        public int? Tonnage { get; set; }

        [Export("Passengers", 7)]
        public int? Passengers { get; set; }

        [Export("Crew", 8)]
        public int? Crew { get; set; }

        [Export("Decks", 9)]
        public int? Decks { get; set; }

        [Export("Cabins", 10)]
        public int? Cabins { get; set; }

        [Export("Name", 11)]
        public string Name { get; set; }

        [Export("Callsign", 12)]
        public string Callsign { get; set; }

        [Export("MMSI", 13)]
        public string MMSI { get; set; }

        [Export("Type", 14)]
        public string VesselType { get; set; }

        [Export("Flag", 15)]
        public string Flag { get; set; }

        [Export("Operator", 16)]
        public string Operator { get; set; }

        public static ExportableVessel FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableVessel
            {
                IMO = words[0].Replace("\"", "").Trim().CleanCode(),
                Built = ExtractInteger(words[1].Replace("\"", "").Trim()),
                Draught = ExtractDecimal(words[2].Replace("\"", "").Trim()),
                Length = ExtractInteger(words[3].Replace("\"", "").Trim()),
                Beam = ExtractInteger(words[4].Replace("\"", "").Trim()),
                Tonnage = ExtractInteger(words[5].Replace("\"", "").Trim()),
                Passengers = ExtractInteger(words[6].Replace("\"", "").Trim()),
                Crew = ExtractInteger(words[7].Replace("\"", "").Trim()),
                Decks = ExtractInteger(words[8].Replace("\"", "").Trim()),
                Cabins = ExtractInteger(words[9].Replace("\"", "").Trim()),
                Name = words[10].Replace("\"", "").Trim(),
                Callsign = words[11].Replace("\"", "").Trim().CleanCode(),
                MMSI = words[12].Replace("\"", "").Trim().CleanCode(),
                VesselType = words[13].Replace("\"", "").Trim().TitleCase(),
                Flag = words[14].Replace("\"", "").Trim().CleanCode(),
                Operator = words[15].Replace("\"", "").Trim().TitleCase()
            };
        }

        private static decimal? ExtractDecimal(string word)
            => string.IsNullOrEmpty(word) ? null : decimal.Parse(word);

        private static int? ExtractInteger(string word)
            => string.IsNullOrEmpty(word) ? null : int.Parse(word);
    }
}

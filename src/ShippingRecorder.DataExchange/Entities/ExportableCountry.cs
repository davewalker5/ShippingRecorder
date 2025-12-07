using System;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Attributes;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableCountry : ExportableEntityBase
    {
        /// <summary>
        /// Country Code
        /// Country Name
        /// </summary>
        public const string CsvRecordPattern = @"^""[A-Za-z]{2}"",""(?!\s*"")[\s\S]*"".?$";

        [Export("Code", 1)]
        public string Code { get; set; }

        [Export("Name", 1)]
        public string Name { get; set; }

        public static ExportableCountry FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableCountry
            {
                Code = words[0].Replace("\"", "").Trim().CleanCode(),
                Name = words[1].Replace("\"", "").Trim().TitleCase()
            };
        }
    }
}

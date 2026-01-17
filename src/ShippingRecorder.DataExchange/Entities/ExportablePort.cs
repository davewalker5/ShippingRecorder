using System;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Attributes;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportablePort : ExportableEntityBase
    {
        /// <summary>
        /// Country Code
        /// UN/LOCODE
        /// Port Name
        /// </summary>
        public const string CsvRecordPattern = @"^""[A-Za-z]{2}[A-Za-z0-9]{3}"",""(?!\s*"")[\s\S]*"".?$";

        [Export("Code", 1)]
        public string Code { get; set; }

        [Export("Name", 2)]
        public string Name { get; set; }

        public static ExportablePort FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportablePort
            {
                Code = words[0].Replace("\"", "").Trim().CleanCode(),
                Name = words[1].Replace("\"", "").Trim().TitleCase()
            };
        }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.BusinessLogic.Extensions;
using ShippingRecorder.DataExchange.Attributes;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Entities
{
    [ExcludeFromCodeCoverage]
    public class ExportableVesselType : ExportableEntityBase, INamedEntity
    {
        /// <summary>
        /// Type Name
        /// </summary>
        public const string CsvRecordPattern = @"^""(?!\s*"")[\s\S]*"".?$";

        [Export("Name", 1)]
        public string Name { get; set; }

        public static ExportableVesselType FromCsv(string record)
        {
            string[] words = record.Split(["\",\""], StringSplitOptions.None);
            return new ExportableVesselType
            {
                Name = words[0].Replace("\"", "").Trim().TitleCase()
            };
        }
    }
}

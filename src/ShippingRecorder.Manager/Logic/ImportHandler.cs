using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.Manager.Logic
{
    internal class ImportHandler : CommandHandlerBase
    {
        public ImportHandler(
            ShippingRecorderApplicationSettings settings,
            ManagerCommandLineParser parser,
            IShippingRecorderFactory factory) : base (settings, parser, factory)
        {

        }

        /// <summary>
        /// Handle the countries import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleCountryImportAsync()
        {
            var filePath = Parser.GetValues(CommandLineOptionType.ImportCountries)[0];
            var importer = new CountryImporter(Factory, ExportableCountry.CsvRecordPattern);
            await importer.ImportAsync(filePath);
        }

        /// <summary>
        /// Handle the operators import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleOperatorImportAsync()
        {
            var filePath = Parser.GetValues(CommandLineOptionType.ImportOperators)[0];
            var importer = new OperatorImporter(Factory, ExportableOperator.CsvRecordPattern);
            await importer.ImportAsync(filePath);
        }

        /// <summary>
        /// Handle the port import command
        /// </summary>
        /// <returns></returns>
        public async Task HandlePortImportAsync()
        {
            var filePath = Parser.GetValues(CommandLineOptionType.ImportPorts)[0];
            var importer = new PortImporter(Factory, ExportablePort.CsvRecordPattern);
            await importer.ImportAsync(filePath);
        }

        /// <summary>
        /// Handle the vessel type import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVesselTypeImportAsync()
        {
            var filePath = Parser.GetValues(CommandLineOptionType.ImportVesselTypes)[0];
            var importer = new VesselTypeImporter(Factory, ExportableVesselType.CsvRecordPattern);
            await importer.ImportAsync(filePath);
        }

        /// <summary>
        /// Handle the vessels import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVesselImportAsync()
        {
            var filePath = Parser.GetValues(CommandLineOptionType.ImportVessels)[0];
            var importer = new VesselImporter(Factory, ExportableVessel.CsvRecordPattern);
            await importer.ImportAsync(filePath);
        }
    }
}
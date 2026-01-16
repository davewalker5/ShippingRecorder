using System;
using System.IO;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Import;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Jobs;

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
        /// Generic import method that also creates job status records for imports
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="type"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private async Task HandleImport<T, U>(CommandLineOptionType type, string pattern)
            where T : CsvImporter<U>
            where U: ExportableEntityBase
        {
            // Get the import file path
            var filePath = Parser.GetValues(type)[0];

            // Synthesise a work item identical to the ones used by the API for data exchange. This results in
            // entries written to the status table having the same formatting as those writen by the API
            var entityName = typeof(U).Name.Replace("Exportable", "");
            var workItem = new ImportWorkItem
            {
                JobName = $"{entityName} Import",
                FileName = Path.GetFileName(filePath)
            };

            // Create the job status record
            var jobStatus = await Factory.JobStatuses.AddAsync(workItem.JobName, workItem.ToString());

            try
            {
                // Create the importer, import the data and update the job status for completion with no error
                var importer = (T)Activator.CreateInstance(typeof(T), Factory, pattern);
                await importer.ImportAsync(filePath);
                await Factory.JobStatuses.UpdateAsync(jobStatus.Id, null);
            }
            catch (Exception ex)
            {
                // Update the job status for completion with errors
                await Factory.JobStatuses.UpdateAsync(jobStatus.Id, ex.Message);
            }
        }

        /// <summary>
        /// Handle the countries import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleCountryImportAsync()
            => await HandleImport<CountryImporter, ExportableCountry>(CommandLineOptionType.ImportCountries, ExportableCountry.CsvRecordPattern);


        /// <summary>
        /// Handle the locations import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleLocationImportAsync()
            => await HandleImport<LocationImporter, ExportableLocation>(CommandLineOptionType.ImportLocations, ExportableLocation.CsvRecordPattern);

        /// <summary>
        /// Handle the operators import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleOperatorImportAsync()
            => await HandleImport<OperatorImporter, ExportableOperator>(CommandLineOptionType.ImportOperators, ExportableOperator.CsvRecordPattern);

        /// <summary>
        /// Handle the port import command
        /// </summary>
        /// <returns></returns>
        public async Task HandlePortImportAsync()
            => await HandleImport<PortImporter, ExportablePort>(CommandLineOptionType.ImportPorts, ExportablePort.CsvRecordPattern);

        /// <summary>
        /// Handle the vessel type import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVesselTypeImportAsync()
            => await HandleImport<VesselTypeImporter, ExportableVesselType>(CommandLineOptionType.ImportVesselTypes, ExportableVesselType.CsvRecordPattern);

        /// <summary>
        /// Handle the vessels import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVesselImportAsync()
            => await HandleImport<VesselImporter, ExportableVessel>(CommandLineOptionType.ImportVessels, ExportableVessel.CsvRecordPattern);

        /// <summary>
        /// Handle the sightings import command
        /// </summary>
        /// <returns></returns>
        public async Task HandleSightingImportAsync()
            => await HandleImport<SightingImporter, ExportableSighting>(CommandLineOptionType.ImportSightings, ExportableSighting.CsvRecordPattern);
    }
}
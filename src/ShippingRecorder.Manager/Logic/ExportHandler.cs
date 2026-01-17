using System;
using System.IO;
using System.Threading.Tasks;
using ShippingRecorder.BusinessLogic.Config;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Export;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Config;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Jobs;

namespace ShippingRecorder.Manager.Logic
{
    internal class ExportHandler : CommandHandlerBase
    {
        public ExportHandler(
            ShippingRecorderApplicationSettings settings,
            ManagerCommandLineParser parser,
            IShippingRecorderFactory factory) : base (settings, parser, factory)
        {

        }

        /// <summary>
        /// Generic export method that also creates job status records for exports
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="type"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        private async Task HandleExport<T, E, D>(CommandLineOptionType type)
            where T : IExporter<E, D>
            where E : ExportableEntityBase
            where D : ShippingRecorderEntityBase
        {
            // Get the export file path
            var filePath = Parser.GetValues(type)[0];

            // Synthesise a work item identical to the ones used by the API for data exchange. This results in
            // entries written to the status table having the same formatting as those writen by the API
            var entityName = typeof(D).Name;
            var workItem = new ExportWorkItem
            {
                JobName = $"{entityName} Export",
                FileName = Path.GetFileName(filePath)
            };

            // Create the job status record
            var jobStatus = await Factory.JobStatuses.AddAsync(workItem.JobName, workItem.ToString());

            try
            {
                // Create the importer, import the data and update the job status for completion with no error
                var exporter = (T)Activator.CreateInstance(typeof(T), Factory);
                await exporter.ExportAsync(filePath);
                await Factory.JobStatuses.UpdateAsync(jobStatus.Id, null);
            }
            catch (Exception ex)
            {
                // Update the job status for completion with errors
                await Factory.JobStatuses.UpdateAsync(jobStatus.Id, ex.Message);
            }
        }

        /// <summary>
        /// Handle the country export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleCountryExportAsync()
            => await HandleExport<CountryExporter, ExportableCountry, Country>(CommandLineOptionType.ExportCountries);

        /// <summary>
        /// Handle the location export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleLocationExportAsync()
            => await HandleExport<LocationExporter, ExportableLocation, Location>(CommandLineOptionType.ExportLocations);

        /// <summary>
        /// Handle the operator export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleOperatorExportAsync()
            => await HandleExport<OperatorExporter, ExportableOperator, Operator>(CommandLineOptionType.ExportOperators);

        /// <summary>
        /// Handle the port export command
        /// </summary>
        /// <returns></returns>
        public async Task HandlePortExportAsync()
            => await HandleExport<PortExporter, ExportablePort, Port>(CommandLineOptionType.ExportPorts);

        /// <summary>
        /// Handle the sightings export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleSightingExportAsync()
            => await HandleExport<SightingExporter, ExportableSighting, Sighting>(CommandLineOptionType.ExportSightings);

        /// <summary>
        /// Handle the vessel export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVesselExportAsync()
            => await HandleExport<VesselExporter, ExportableVessel, Vessel>(CommandLineOptionType.ExportVessels);

        /// <summary>
        /// Handle the vessel type export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVesselTypeExportAsync()
            => await HandleExport<VesselTypeExporter, ExportableVesselType, VesselType>(CommandLineOptionType.ExportVesselTypes);

        /// <summary>
        /// Handle the voyage export command
        /// </summary>
        /// <returns></returns>
        public async Task HandleVoyageExportAsync()
            => await HandleExport<VoyageExporter, ExportableVoyage, Voyage>(CommandLineOptionType.ExportVoyages);
    }
}
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using ShippingRecorder.Entities.Db;

namespace ShippingRecorder.DataExchange.Export
{
    public class NamedEntityExporter<E, D> : INamedEntityExporter<E, D>
        where E : ExportableEntityBase, INamedEntity
        where D : ShippingRecorderEntityBase, INamedEntity
    {
        protected readonly IShippingRecorderFactory _factory;

        public event EventHandler<ExportEventArgs<E>> RecordExport;

        public NamedEntityExporter(IShippingRecorderFactory factory)
            => _factory = factory;

        /// <summary>
        /// Export a collection of named entities to a CSV file
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="file"></param>
#pragma warning disable CS1998
        public async Task ExportAsync(IEnumerable<D> entities, string file)
        {
            // Convert the entities to exportable (flattened hierarchy) entities
            List<E> exportableEntities = [];
            foreach (var entity in entities)
            {
                var exportable = (E)Activator.CreateInstance(typeof(E));
                exportable.Name = entity.Name;
                exportableEntities.Add(exportable);
            }

            // Configure an exporter to export them
            var exporter = new CsvExporter<E>(ExportableEntityBase.DateFormat);
            exporter.RecordExport += OnRecordExported;

            // Export the records
            exporter.Export(exportableEntities, file, ',');
        }
#pragma warning restore CS1998

        /// <summary>
        /// Handler for vessel export notifications
        /// </summary>
        /// <param name="_"></param>
        /// <param name="e"></param>
        private void OnRecordExported(object _, ExportEventArgs<E> e)
        {
            RecordExport?.Invoke(this, e);
        }
    }
}

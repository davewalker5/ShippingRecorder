using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingRecorder.DataExchange.Entities;
using ShippingRecorder.DataExchange.Interfaces;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;

namespace ShippingRecorder.DataExchange.Import
{
    public sealed class OperatorImporter : CsvImporter<ExportableOperator>, IOperatorImporter
    {
        private List<Operator> _operators;

        public OperatorImporter(IShippingRecorderFactory factory, string format) : base(factory, format)
        {

        }

        /// <summary>
        /// Prepare for import
        /// </summary>
        /// <returns></returns>
        protected override async Task Prepare()
        {
            await base.Prepare();
            _operators = await _factory.Operators.ListAsync(x => true, 1, int.MaxValue).ToListAsync();
        }

        /// <summary>
        /// Inflate a record to an object
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override ExportableOperator Inflate(string record)
            => ExportableOperator.FromCsv(record);

        /// <summary>
        /// Validate an inflated record
        /// </summary>
        /// <param name="op"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
#pragma warning disable CS1998
        protected override void Validate(ExportableOperator op, int recordCount)
        {
            ValidateField<string>(x => CheckOperatorDoesNotExist(x), op.Name,  "Name", recordCount);
        }
#pragma warning restore CS1998

        /// <summary>
        /// Store an inflated record in the database
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        protected override async Task AddAsync(ExportableOperator op)
            => await _factory.Operators.AddAsync(op.Name);

        /// <summary>
        /// Check an operator does not exist
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool CheckOperatorDoesNotExist(string name)
            => _operators.FirstOrDefault(x => x.Name == name) == null;
    }
}
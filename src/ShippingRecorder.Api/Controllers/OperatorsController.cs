using ShippingRecorder.BusinessLogic.Factory;
using ShippingRecorder.Entities.Db;
using ShippingRecorder.Entities.Interfaces;
using ShippingRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShippingRecorder.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class OperatorsController : ShippingRecorderApiController
    {
        public OperatorsController(ShippingRecorderFactory factory, IShippingRecorderLogger logger) : base(factory, logger)
        {
        }

        [HttpGet]
        [Route("{pageNumber}/{pageSize}")]
        public async Task<ActionResult<List<Operator>>> GetOperatorsAsync(int pageNumber, int pageSize)
        {
            LogMessage(Severity.Debug, $"Retrieving list of operators (page {pageNumber}, page size {pageSize})");

            List<Operator> operators = await Factory.Operators.ListAsync(x => true, pageNumber, pageSize).ToListAsync();

            LogMessage(Severity.Debug, $"Retrieved {operators.Count} operator(s)");

            if (!operators.Any())
            {
                return NoContent();
            }

            return operators;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Operator>> GetOperatorAsync(int id)
        {
            LogMessage(Severity.Debug, $"Retrieving operator with ID {id}");

            Operator op = await Factory.Operators.GetAsync(m => m.Id == id);

            if (op == null)
            {
                LogMessage(Severity.Debug, $"Operator with ID {id} not found");
                return NotFound();
            }

            return op;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Operator>> AddOperatorAsync([FromBody] Operator template)
        {
            LogMessage(Severity.Debug, $"Adding operator: Name = {template.Name}");
            Operator op = await Factory.Operators.AddAsync(template.Name);
            LogMessage(Severity.Debug, $"Operator added: {op}");
            return op;
        }

        [HttpPut]
        [Route("")]
        public async Task<ActionResult<Operator>> UpdateOperatorAsync([FromBody] Operator template)
        {
            LogMessage(Severity.Debug, $"Updating operator: ID = {template.Id}, Name = {template.Name}");
            Operator op = await Factory.Operators.UpdateAsync(template.Id, template.Name);
            LogMessage(Severity.Debug, $"Operator updated: {op}");
            return op;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOperatorAsync(int id)
        {
            LogMessage(Severity.Debug, $"Deleting operator: ID = {id}");
            await Factory.Operators.DeleteAsync(id);
            return Ok();
        }
    }
}

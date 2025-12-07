using ShippingRecorder.Api.Entities;
using ShippingRecorder.Api.Interfaces;
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
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IShippingRecorderLogger _logger;

        public UsersController(IUserService userService, IShippingRecorderLogger logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticateModel model)
        {
            _logger.LogMessage(Severity.Debug, $"Authenticating as {model.UserName}");

            string token = await _userService.AuthenticateAsync(model.UserName, model.Password);

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogMessage(Severity.Debug, $"Authentication failed for {model.UserName}");
                return BadRequest();
            }

            return Ok(token);
        }
    }
}

using InvictaPartnersAPI.Interfaces;
using InvictaPartnersAPI.Models;
using InvictaPartnersAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Controllers
{
    [ApiController]
    [Route("api/authenticate")]
    public class AuthenticationController : ControllerBase
    {
        private IUserService _userService;
        private ILogger<AuthenticationController> _logger;

        private readonly ICosmosDbService _cosmosDbService;
        public AuthenticationController(IUserService userService,ICosmosDbService cosmosDbService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _cosmosDbService = cosmosDbService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> AuthenticateAsync(AuthenticateRequest model)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");

            _logger.LogInformation("Authentication service initializing");
            _logger.LogInformation($"User: {model.Username}");
            var response = await _userService.AuthenticateAsync(model);

            if (response == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(response);
        }

    }
}

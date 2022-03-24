using Microsoft.AspNetCore.Mvc;
using InvictaPartnersAPI.Models;
using InvictaPartnersAPI.Services;
using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using InvictaPartnersAPI.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers
{
    
    [ApiController]
    [Route("api/user")]
    [Authorize]
        public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly ICosmosDbService _cosmosDbService;
        public UsersController(IUserService userService,ICosmosDbService cosmosDbService, ILogger<UsersController> _logger)
        {
            _userService = userService;
            _cosmosDbService = cosmosDbService;
        }


        [HttpGet("{id}")]
        public async Task<Models.User> GetUserAsync(string id) 
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            _logger.LogInformation("id: "+id);
            var jti = GetIdFromToken();
            Models.User _requser = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_requser.customerId);
            
            Models.User _user = await _cosmosDbService.GetUserAsync(id);

            if (_user.id.Equals(_requser.id) || _requser.role.Equals("admin") )
            {
                _user.password = null;
                return _user;
            }
            
            return null;

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUpdateUserAsync(Models.User user)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            if (ModelState.IsValid)
            {

                var jti = GetIdFromToken();
                Models.User _requser = await _cosmosDbService.GetUserAsync(jti);
                _logger.LogInformation("customerId:"+_requser.customerId);


                user.password = UserService.GetHashString(user.password);
                Models.User _user = await _cosmosDbService.GetUserAsync(user.id);

                if (_user == null && _requser.role.Equals("admin") )
                {
                    user.type = "user";
                    user.id = Guid.NewGuid().ToString();
                    await _cosmosDbService.AddUserAsync(user);
                    return StatusCode(StatusCodes.Status201Created);
                } else {
                    if (_requser.id.Equals(_user.id) || _requser.role.Equals("admin")) {

                        if (!_requser.role.Equals("admin")) {
                            user.role="user"; //non admin cannot change role
                        }
                        await _cosmosDbService.UpdateUserAsync(_user.id, user);
                        return StatusCode(StatusCodes.Status202Accepted);
                    }
                }

            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        private string GetIdFromToken()
        {
            string accessToken = HttpContext.Request.Headers["authorization"];
            var newToken = accessToken.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(newToken);
            var tokenS = handler.ReadToken(newToken) as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "id").Value;
            _logger.LogInformation("jti: " + jti);
            return jti;
        }


    }
}

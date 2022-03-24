using InvictaPartnersAPI.Exceptions;
using InvictaPartnersAPI.Interfaces;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ISupplierService _service;
        private readonly ILogger<InventoryController> _logger;
        private readonly ICosmosDbService _cosmosDbService;

        public InventoryController(ISupplierService service, ILogger<InventoryController> logger, ICosmosDbService cosmosDbService)
        {
            _service = service;
            _logger = logger;
            _cosmosDbService = cosmosDbService;
        }

        [Authorize]
        [HttpPost("{supplierId}")]
        public async Task<IActionResult> PostInventory(List<InventoryEntry> items,int supplierId)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");

            _logger.LogInformation("Updating inventory");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);
            if(_user.firstName == "LEGSONS" && supplierId == 8)
            {
                _logger.LogInformation("Service starting, execution performed by LEG&SONS");
                var data = await _service.PostInventory(items, supplierId);
                if(data == true)
                {
                    var r = new
                    {
                        Status = 200,
                        Message = "Inventory updated successfully",
                        Company = "TCO"
                    };
                    return Ok(r);
                }
                else
                {
                    throw new BusinessException("The inventory was not updated, something went wrong");
                }
                
            }
            else
            {
                _logger.LogError("You do not have access to this service");
                throw new BusinessException("You do not have access to this service");
            }
            
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

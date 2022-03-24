using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using InvictaPartnersAPI.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers
{


    [Route("api/price/batch")]
    [ApiController]    public class PriceBatchController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<PriceBatchController> _logger;
        public IConfiguration _configuration;

        public PriceBatchController(ICosmosDbService cosmosDbService,IConfiguration configuration, ILogger<PriceBatchController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateAsyncBatch(List<PriceEntry> priceList)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _requser = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_requser.customerId);
            if(!_requser.role.Equals("admin")) {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            if (ModelState.IsValid)
            {
                foreach(PriceEntry entry in priceList) {
                    PriceEntry _entry = await _cosmosDbService.GetPriceAsync(entry.sku+":"+entry.validDate,entry.type);
                    if (_entry == null)
                    {
                        entry.id = entry.sku+":"+entry.validDate;
                        await _cosmosDbService.AddPriceAsync(entry,entry.type);
                    } else {
                        entry.id = _entry.id;
                        await _cosmosDbService.UpdatePriceAsync(_entry.id, entry);
                    }

                }
                return StatusCode(StatusCodes.Status202Accepted);
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
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


    [Route("api/catalog/batch")]
    [ApiController]    public class ItemBatchController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<ItemBatchController> _logger;
        public IConfiguration _configuration;

        public ItemBatchController(ICosmosDbService cosmosDbService,IConfiguration configuration, ILogger<ItemBatchController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateAsyncBatch(List<Item> itemList)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _requser = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_requser.customerId);
            if(!_requser.role.Equals("admin")) {
                _logger.LogError("You do not have access to this service");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            if (ModelState.IsValid)
            {
                foreach(Item item in itemList) {

                    Double price = 0.0;
                    List<PriceEntry> priceList = await _cosmosDbService.GetLatestPriceAsync(item.sku,"price");
                    if (priceList.Count>0){
                        price=priceList[0].unitCost;
                    }
                    Item _item = await _cosmosDbService.GetItemAsync(item.sku,item.type);
                    if (_item == null)
                    {
                        item.unitCost=price;
                        item.id = item.sku.Replace("/","-");
                        await _cosmosDbService.AddItemAsync(item,item.type);
                    } else {
                        item.id = _item.id;
                        item.unitCost=price;
                        await _cosmosDbService.UpdateItemAsync(_item.id, item);
                    }

                }
                _logger.LogInformation("Completed process");
                return StatusCode(StatusCodes.Status202Accepted);
            }
            _logger.LogError("Process failure ");
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
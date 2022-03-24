using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using InvictaPartnersAPI.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers { 

    [Route("api/tco/order")]
    [ApiController]

    public class OrderSellwareController : Controller
{
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<OrderSellwareController> _logger;
        public OrderSellwareController(ICosmosDbService cosmosDbService, ILogger<OrderSellwareController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;

        }


        [HttpGet]
        [Authorize]
        public async Task<List<SellwareOrder>> ViewAsync(string status)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);

            var sql = "SELECT * FROM c WHERE c.type='tco/order' ";

            if (status!=null) {
                sql=sql+ " and c.status = '"+status+"' ";
            }

            return (List<SellwareOrder>)await _cosmosDbService.GetOrderSellwareAsyncQuery(sql);

            
        }

        [HttpGet("{orderNumber}")]
        [Authorize]
        public async Task<SellwareOrder> ViewAsyncById(string orderNumber)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");

            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);

            var results = await _cosmosDbService.GetOrderSellwareAsync(orderNumber,"tco/order");
            if (results!=null) {
                if (_user.role.Equals("admin") ) {                
                    return results;
                } else {
                    return null;
                }
            } else {
                return null;
            }
            
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UpdateAsync(SellwareOrder order)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_user.customerId);

            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(order.id))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,"Sellware orders need to be created using sellware controller");
                }
                else
                {
                    SellwareOrder _order = await _cosmosDbService.GetOrderSellwareAsync(order.id,"tco/order");
                    if (_order == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        if ( _user.role.Equals("admin")) {                                                                                                            
                            await _cosmosDbService.UpdateOrderSellwareAsync(_order.id, order,"tco/order");
                            //update timestamp of affected items
                            foreach(var orderitem in _order.orderItemList){
                                var item = await _cosmosDbService.GetItemAsync(orderitem.sku,"tco/item");
                                await _cosmosDbService.AddItemAsync(item,"tco/item");
                            }
                            return StatusCode(StatusCodes.Status202Accepted);
                        } else {
                            return StatusCode(StatusCodes.Status400BadRequest);
                        }
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

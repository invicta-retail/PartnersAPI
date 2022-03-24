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

    [Route("api/order")]
    [ApiController]

    public class OrderController : Controller
{
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<OrderController> _logger;
        public OrderController(ICosmosDbService cosmosDbService, ILogger<OrderController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;

        }


        [HttpGet]
        [Authorize]
        public async Task<List<Order>> ViewAsync(string customerId, string status)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_user.customerId);

            var sql = "SELECT * FROM c WHERE c.type='order' ";

            if (status!=null) {
                sql=sql+ " and c.status = '"+status+"' ";
            }
            if (customerId != null && _user.role.Equals("admin") ) {
                sql=sql+ " and c.customerId='" + customerId +"' ";
            } else if (_user.role.Equals("user")) {
                sql=sql+ " and c.customerId='" + _user.customerId +"' ";
            }

            return (List<Order>)await _cosmosDbService.GetOrderAsyncQuery(sql);

            
        }

        [HttpGet("{orderNumber}")]
        [Authorize]
        public async Task<Order> ViewAsyncById(string orderNumber)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_user.customerId);

            var results = await _cosmosDbService.GetOrderAsync(orderNumber,"order");
            if (results!=null) {
                if (_user.role.Equals("admin") || results.customerId.Equals(_user.customerId) ) {                
                    return results;
                } else {
                    return null;
                }
            } else {
                return null;
            }
            
        }

        [HttpDelete("{orderNumber}")]
        [Authorize]
        public async Task<ActionResult> DeleteAsync(string orderNumber)
            {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _user = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogInformation("customerId:"+_user.customerId);

            if (orderNumber == null)
            {
                return BadRequest();
            }

            Order order = await _cosmosDbService.GetOrderAsync(orderNumber,"order");
            if (order == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            if ((_user.role.Equals("admin") || order.customerId.Equals(_user.customerId)) && order.status.Equals("Pending") ) {    
                await _cosmosDbService.DeleteOrderAsync(orderNumber,"order");
                return StatusCode(StatusCodes.Status200OK);
            } else {
                return StatusCode(StatusCodes.Status404NotFound);
            }


        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateAsync(Order order)
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


                    //Validate order does not exists
                    _logger.LogInformation("order.customerRefNumber:"+ order.customerRefNumber);
                    var existingOrder = await _cosmosDbService.GetOrderAsyncQuery("SELECT * FROM c where c.type = 'order' and c.customerRefNumber='"+order.customerRefNumber+"'");
                    if (existingOrder.Count()>0) {

                        return StatusCode(StatusCodes.Status400BadRequest, "{\"message\": \"Order already exist, customerRefNumber: "+order.customerRefNumber+"\"}");
                    };


                    //default values
                    Sequence _sequence = await _cosmosDbService.GetSequenceAsync("order");
                    order.type = "order";
                    order.id = _sequence.sequence.ToString();
                    order.orderNumber = _sequence.sequence.ToString();
                    order.orderDate = DateTime.Now;
                    order.status = "Pending";
                    order.packages = null;

                    //validation
                    if (order.products == null)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "{\"message\": \"order requires products\"}");
                    }

                    //IWE values
                    if (!order.customerId.Equals(_user.customerId) && !_user.role.Equals("admin"))
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "{\"message\": \"invalid customer id\"}");
                    }
                    else
                    {
                        //address and customer required for internal orders
                        if (_user.defaulShippingAddress) {
                            order.customerName = _user.company;
                            order.customerEmail = _user.email;

                            Address _shippingAddress = new Address();
                            _shippingAddress.phone = _user.phone;
                            _shippingAddress.email = _user.email;
                            _shippingAddress.name = _user.company;
                            _shippingAddress.company = _user.company;
                            _shippingAddress.addressLine1 =_user.addressLine1;
                            _shippingAddress.addressLine2 =_user.addressLine2;
                            _shippingAddress.postalCode = _user.postalCode;
                            _shippingAddress.city = _user.city;;
                            _shippingAddress.state = _user.state;
                            _shippingAddress.countryCode = _user.countryCode;

                            order.shippingAddress = _shippingAddress;
                        }
                    }                    

                    //catalog validation
                    List<OrderItem> consolidatedProductsList = new List<OrderItem>();
                    if (order.products != null)
                    {
                        foreach (OrderItem item in order.products)
                        {
                            Item itemCatalog = await _cosmosDbService.GetItemAsync(item.sku,"item");
                            if (itemCatalog != null)
                            {
                                var foundItem = consolidatedProductsList.Find(p=> p.sku == item.sku);
                                item.unitPrice = itemCatalog.unitCost;
                                item.name = itemCatalog.name;
                                item.currency = itemCatalog.currency;
                                item.status = "Pending";
                                item.quantityShip = 0;
                                item.quantityCanceled = 0;

                                if (foundItem==null) {
                                    consolidatedProductsList.Add(item);
                                } else {
                                    foundItem.quantityOrdered = foundItem.quantityOrdered+item.quantityOrdered;
                                }

                            }
                            else
                            {
                                item.status = "Not in Catalog";
                                item.quantityCanceled = item.quantityOrdered;
                                consolidatedProductsList.Add(item);
                            }

                        }
                    }
                    order.products=consolidatedProductsList;
                    await _cosmosDbService.AddOrderAsync(order,"order");
                    return StatusCode(StatusCodes.Status201Created, order);
                }
                else
                {
                    Order _order = await _cosmosDbService.GetOrderAsync(order.id,"order");
                    if (_order == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        if ( _user.role.Equals("admin")) {                                                                                                            
                            await _cosmosDbService.UpdateOrderAsync(_order.id, order,"order");
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

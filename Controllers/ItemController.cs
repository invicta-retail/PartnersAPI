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
using InvictaPartnersAPI.Exceptions;

namespace InvictaPartnersAPI.Controllers
{


    [Route("api/catalog")]
    [ApiController]    public class ItemController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<ItemController> _logger;
        public IConfiguration _configuration;

        public ItemController(ICosmosDbService cosmosDbService,IConfiguration configuration, ILogger<ItemController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpGet]
        [Authorize]
        public async Task<List<Item>> ViewAsync() {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var commitmentItemList = await GetSales();
            _logger.LogInformation($"Sales found: {commitmentItemList.Count}");
            TimeStamp  catalogTimeStamp = await _cosmosDbService.GetTimeStampAsync("catalog");
            _logger.LogInformation($"Timestamp {catalogTimeStamp.timestamp}");
            Int32 timevalue = catalogTimeStamp.timestamp;
            _logger.LogInformation($"Timevalue {timevalue}");
            var results = (List<Item>)await _cosmosDbService.GetItemsAsync("SELECT * FROM c where c.type = 'item' and c.unitCost>0 and c.totalAvailableQuantity>0 and c._ts>="+timevalue );
            
            foreach(var item in results){
                int oh = item.totalAvailableQuantity - GetSkuSales(commitmentItemList,item.sku);
                if (oh<0) {
                    oh = 0;
                }
                item.totalAvailableQuantity=oh;
            }
            _logger.LogInformation("Completed process successfully");
            return results;
        }

        [HttpGet("{sku}")]
        [Authorize]
        public async Task<Item> ViewAsyncById(string sku)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var commitmentItemList = await GetSales();
            _logger.LogInformation($"Sales found: {commitmentItemList.Count}");
            TimeStamp  catalogTimeStamp = await _cosmosDbService.GetTimeStampAsync("catalog");
            _logger.LogInformation($"Timestamp {catalogTimeStamp.timestamp}");
            Int32 timevalue = catalogTimeStamp.timestamp;
            _logger.LogInformation($"Timevalue {timevalue}");
            _logger.LogInformation($"Getting item {sku}");
            var results = await _cosmosDbService.GetItemAsync(sku,"item");
            if (results._ts < timevalue) {
                results.totalAvailableQuantity=0;
            }

            if(results.unitCost==0){
                results.totalAvailableQuantity=0;
            }

            int oh = results.totalAvailableQuantity - GetSkuSales(commitmentItemList,results.sku);
            if (oh<0) {
                oh = 0;
            }
            results.totalAvailableQuantity=oh;
            _logger.LogDebug($"Results : {results.sku}");
            return results;
            
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> DeleteAsync(string sku)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");

            var jti = GetIdFromToken();
            Models.User _requser = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogDebug("customerId:"+_requser.customerId);
            if(!_requser.role.Equals("admin")) {
                _logger.LogError("You do not have access to this service");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            if (sku == null)
            {
                _logger.LogError("Invalid SKU");
                throw new BusinessException("Invalid SKU");
            }

            if (sku.Equals("all")) {

                dynamic[] newItems = new dynamic[]
                {
                };

                var section = _configuration.GetSection("CosmosDb");
                _logger.LogDebug("Account: "+section.GetSection("Account").Value);
                _logger.LogDebug("Key: "+section.GetSection("Key").Value);

                var client = new CosmosClient(section.GetSection("Account").Value, section.GetSection("Key").Value);

                var result = await client.GetContainer(section.GetSection("DatabaseName").Value, section.GetSection("ContainerName").Value).Scripts.ExecuteStoredProcedureAsync<string>("BulkDelete", new PartitionKey("item"), new[] { newItems });

                _logger.LogDebug("Delete All Result:"+ result);
                return StatusCode(StatusCodes.Status200OK);

            } else {
                Item item = await _cosmosDbService.GetItemAsync(sku,"item");
                if (item == null)
                {
                    _logger.LogDebug("Failed process");
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                _logger.LogDebug("Completed process");
                await _cosmosDbService.DeleteItemAsync(sku,"item");
                return StatusCode(StatusCodes.Status200OK);

            }

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateAsync([Bind("sku,active,name,totalAvailableQuantity,currency,unitCost,sourceCode")] Item item)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var jti = GetIdFromToken();
            Models.User _requser = await _cosmosDbService.GetUserAsync(jti);
            _logger.LogDebug("customerId:"+_requser.customerId);
            if(!_requser.role.Equals("admin")) {
                _logger.LogError("You do not have access to this service");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            if (ModelState.IsValid)
            {

                Item _item = await _cosmosDbService.GetItemAsync(item.sku,"item");
                if (_item == null)
                {
                    item.id = item.sku.Replace("/","-");
                    await _cosmosDbService.AddItemAsync(item,"item");
                    _logger.LogDebug("Completed process");
                    return StatusCode(StatusCodes.Status201Created);
                } else {
                    item.id = _item.id;
                    await _cosmosDbService.UpdateItemAsync(_item.id, item);
                    _logger.LogDebug("Completed process");
                    return StatusCode(StatusCodes.Status202Accepted);
                }

            }
            throw new BusinessException("Invalid Modelstate ");
        }

        private string GetIdFromToken()
        {
            string accessToken = HttpContext.Request.Headers["authorization"];
            var newToken = accessToken.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(newToken);
            var tokenS = handler.ReadToken(newToken) as JwtSecurityToken;
            var jti = tokenS.Claims.First(claim => claim.Type == "id").Value;
            _logger.LogDebug("jti: " + jti);
            return jti;
        }

        private int GetSkuSales(List<ItemCommitment> commitmentList, string sku) {

            var skuCommitment = commitmentList.Find(i => i.sku==sku);
            if (skuCommitment==null) {
                return 0;
            } else {
                return skuCommitment.commited;
            }            

        }

        private async Task<List<ItemCommitment>> GetSales()
        {
            var queryString = "select * from c where c.type = 'order' and c.status not in ('Canceled','Complete') ";

            var pendingSales = (List<Order>) await _cosmosDbService.GetOrderAsyncQuery(queryString);

            List<ItemCommitment> commitmentItemList = new List<ItemCommitment>();

            foreach( var order in pendingSales ) {
                foreach( var item in order.products) {

                    var skuCommitment = commitmentItemList.Find(i => i.sku == item.sku);
                    if (skuCommitment==null) {
                        
                        ItemCommitment newCommitment = new ItemCommitment(){
                            sku=item.sku,
                            commited=item.quantityOrdered-item.quantityCanceled-item.quantityShip
                        };

                        commitmentItemList.Add(newCommitment);

                    } else {
                        skuCommitment.commited += item.quantityOrdered-item.quantityCanceled-item.quantityShip;
                    }

                }
            }
            return commitmentItemList;

        }


    }
}
using InvictaPartnersAPI.Exceptions;
using InvictaPartnersAPI.Interfaces;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Controllers
{

    [Route("api/tco/sellware")]
    [ApiController]

    public class SellwareController : Controller
{
        private readonly ICosmosDbService _cosmosDbService;
        private readonly ILogger<SellwareController> _logger;
        private readonly IHttpContextAccessor _accessor;
        public SellwareController(ICosmosDbService cosmosDbService, ILogger<SellwareController> logger, IHttpContextAccessor accessor)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _accessor = accessor;
        }


        [HttpPost]
        public async Task<ActionResult> PostOrdersAsync(String abx, SellwareRoot root)
        {
            var address = _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");

            _logger.LogInformation("Analyzing headers");
            //**** Authorization Logic ******//
            String secret = "Cn9wgXJ4a_?#na_8";
            _logger.LogInformation($"Secret {secret}");
            String sellerId = Request.Headers["AuctionBlox-SellerId"];
            _logger.LogInformation($"SellerId {sellerId}");
            String action =  Request.Headers["AuctionBlox-Execute"];
            _logger.LogInformation($"Action {action}");
            String version = Request.Headers["AuctionBlox-Version"];
            _logger.LogInformation($"Version {version}");
            String timestamp = Request.Headers["AuctionBlox-Timestamp"];
            _logger.LogInformation($"Timestamp {timestamp}");
            String signature = Request.Headers["AuctionBlox-Signature"];
            _logger.LogInformation($"Signature {signature}");

            String verify = sellerId+action+version+timestamp+secret;
                // Create a new instance of the MD5CryptoServiceProvider object.

            if (signature != getMd5Hash(verify)) {
                _logger.LogError("You do not have access to this service");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            // var timestamputc = DateTimeOffset.Parse(timestamp).UtcDateTime;
            // var currenttimeutc = DateTime.UtcNow;
            // var timediff = currenttimeutc-timestamputc;
            // if (timediff > new TimeSpan (0,15,0)){
            //     return StatusCode(StatusCodes.Status401Unauthorized); //expired request
            // }

            _logger.LogInformation("Approved access");
            if (ModelState.IsValid && abx.Equals("orders_import"))
            {
                _logger.LogInformation($"User action: {abx}");
                List<OrderResponse> responseList = new List<OrderResponse>();
                _logger.LogInformation($"Response list: {responseList.Count}");
                foreach(var order in root.order) {            

                    Sequence _sequence = await _cosmosDbService.GetSequenceAsync("ordertco");
                    order.id = _sequence.sequence.ToString();
                    order.type = "tco/order";
                    order.status = "Pending";
                    order.merchantOrderNumber = _sequence.sequence.ToString();
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);

                    var orderValidation = ValidateSellwareOrder(order);
            
                    if (orderValidation.valid) {

                        var existingOrder = await _cosmosDbService.GetOrderSellwareAsyncQuery("SELECT * FROM c where c.type = 'tco/order' and c.auctionbloxOrderNumber='"+order.auctionbloxOrderNumber+"'");
                        if (existingOrder.Count()>0) {
                            Error _error = new Error(){code="100",severity="Error",message="Order already exist."};
                            OrderResponse _response = new OrderResponse(){
                                auctionbloxOrderNumber = order.auctionbloxOrderNumber,
                                merchantOrderNumber=existingOrder.FirstOrDefault().merchantOrderNumber,
                                error = _error                   
                            };
                            responseList.Add(_response);   
                        } else {
                        
                            //update timestamp of affected items
                            bool validitem = true;
                            foreach(var orderitem in order.orderItemList){
                                try{
                                    var item = await _cosmosDbService.GetItemAsync(orderitem.sku,"tco/item");
                                    if (item == null) {

                                        Error _error = new Error(){code="110",severity="Error",message="Invalid sku in order: "+orderitem.sku };
                                        OrderResponse _response = new OrderResponse(){
                                            auctionbloxOrderNumber = order.auctionbloxOrderNumber,
                                            error = _error                   
                                        };
                                        responseList.Add(_response); 
                                        validitem = false;
                                    } else { 
                                       orderitem.sourceCode = item.sourceCode;                                       
                                       await _cosmosDbService.UpdateItemAsync(item.id,item);
                                    }

                                } catch (Exception e) {
                                    _logger.LogError(e.ToString());
                                }
                            }
                            if (!validitem) {continue;} //skip order

                            // Save order
                            await _cosmosDbService.AddOrderSellwareAsync(order,"tco/order");
                                OrderResponse response = new OrderResponse(){
                                auctionbloxOrderNumber = order.auctionbloxOrderNumber,
                                merchantOrderNumber = order.id                        
                            };
                            responseList.Add(response);   


                        }

                    } else {
                        OrderResponse response = new OrderResponse(){
                            auctionbloxOrderNumber = order.auctionbloxOrderNumber,
                            error = orderValidation.error                    
                        };
                        responseList.Add(response);         
                    }

                }
                return StatusCode(StatusCodes.Status200OK, responseList);
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        private OrderValidation ValidateSellwareOrder(SellwareOrder order)
        {
     
            OrderValidation validation = new OrderValidation(){
                valid=true
            };
            return validation;
        }

        [HttpGet]
        public async Task<ActionResult> GetOrdersAsync(String abx, String updatedAfter, String sku, String perPage, String page, String timeStart, String timeEnd)
        {

            var address = _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            List<Models.OrderFeed> orderList= new List<Models.OrderFeed>();

            _logger.LogInformation("abx: "+abx);
            _logger.LogInformation("updatedAfter: "+updatedAfter);

            //**** Authorization Logic ******//
            String secret = "Cn9wgXJ4a_?#na_8";
            _logger.LogInformation($"Secret {secret}");
            String sellerId = Request.Headers["AuctionBlox-SellerId"];
            _logger.LogInformation($"SellerId {sellerId}");
            String action = Request.Headers["AuctionBlox-Execute"];
            _logger.LogInformation($"Action {action}");
            String version = Request.Headers["AuctionBlox-Version"];
            _logger.LogInformation($"Version {version}");
            String timestamp = Request.Headers["AuctionBlox-Timestamp"];
            _logger.LogInformation($"Timestamp {timestamp}");
            String signature = Request.Headers["AuctionBlox-Signature"];
            _logger.LogInformation($"Signature {signature}");

            String verify = sellerId+action+version+timestamp+secret;

            if (signature != getMd5Hash(verify)) {
                _logger.LogError("You do not have access to this service");
                return StatusCode(StatusCodes.Status401Unauthorized);  //invalid signature
            }
            // var timestamputc = DateTimeOffset.Parse(timestamp).UtcDateTime;
            // var currenttimeutc = DateTime.UtcNow;
            // var timediff = currenttimeutc-timestamputc;
            // if (timediff > new TimeSpan (0,15,0)){
            //     return StatusCode(StatusCodes.Status401Unauthorized); //expired request
            // }

            int epochTimeStart = 0;  
            if (!String.IsNullOrEmpty(timeStart)) {

                try {
                    var afterDate = DateTimeOffset.Parse(timeStart).UtcDateTime;
                    _logger.LogInformation("TimeStart:"+afterDate);

                    epochTimeStart = (int)(afterDate- new DateTime(1970, 1, 1)).TotalSeconds;
                    _logger.LogInformation("epochTimeStart:"+epochTimeStart);
                } catch (Exception e) {
                    _logger.LogError("Epoch Exception:"+ e.ToString());
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }

            int epochTimeEnd = 0;  
            if (!String.IsNullOrEmpty(timeEnd)) {
                try {
                    var afterDate = DateTimeOffset.Parse(timeEnd).UtcDateTime;
                    _logger.LogInformation("TimeEnd:"+afterDate);

                    epochTimeEnd = (int)(afterDate- new DateTime(1970, 1, 1)).TotalSeconds;
                    _logger.LogInformation("epochTimeEnd:"+epochTimeEnd);
                } catch (Exception e) {
                    _logger.LogError("Epoch Exception:"+ e.ToString());
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }

            if (abx.Equals("get_inventory")) 
            {                

                var commitmentItemList = await GetSellewareSales();

                TimeStamp  catalogTimeStamp = await _cosmosDbService.GetTimeStampAsync("tcocatalog");
                Int32 timevalue = catalogTimeStamp.timestamp;
                int sales = 0;
                
                if (!String.IsNullOrEmpty(sku)) {

                    var results = await _cosmosDbService.GetItemAsync(sku,"tco/item");
                    if(results == null)
                    {
                        throw new BusinessException("Item not found in inventory");
                    }
                    if (results._ts < timevalue) {
                        results.totalAvailableQuantity=0;
                    } else {
                        sales =  GetSkuSellwareSales(commitmentItemList,sku);
                    }
                     int oh = results.totalAvailableQuantity-sales;
                    if (oh <0) {
                        oh = 0;
                    }
                    List<SellwareItem> itemList = new List<SellwareItem>();                

                    itemList.Add(new SellwareItem(){sku=results.sku, quantity = oh, status = results.active ? 1:0});

                    return StatusCode(StatusCodes.Status200OK, itemList);                 
                } else {

                    if (String.IsNullOrEmpty(page)) {
                        page = "0";
                    }

                    if (String.IsNullOrEmpty(perPage)){
                        perPage= "100";
                    }

                    List<SellwareItem> itemList = new List<SellwareItem>();   
                                         
                    var resultsCount = (List<Item>)await _cosmosDbService.GetItemsAsync("SELECT * FROM c where c.type = 'tco/item' and " + (epochTimeStart > 0 ? "c._ts>="+epochTimeStart :"c._ts>="+timevalue) + (epochTimeEnd>0? " and c._ts<= "+epochTimeEnd:" ")  );
                    var results = (List<Item>)await _cosmosDbService.GetItemsAsync("SELECT * FROM c where c.type = 'tco/item' and "+ (epochTimeStart > 0 ? "c._ts>="+epochTimeStart :"c._ts>="+timevalue) + (epochTimeEnd>0? " and c._ts<= "+epochTimeEnd:" ")  + " OFFSET "+Int16.Parse(page)*Int16.Parse(perPage)+" LIMIT "+ Int16.Parse(perPage));


                    foreach(var item in results) {

                        sales = 0;
                        if (item._ts < timevalue) {
                            item.totalAvailableQuantity=0;
                        } else {
                            sales = GetSkuSellwareSales(commitmentItemList,item.sku);
                        }
                    
                        int oh = item.totalAvailableQuantity-sales;
                        if (oh <0) {
                            oh = 0;
                        }

                        itemList.Add(new SellwareItem(){sku=item.sku, quantity = oh, status = item.active ? 1:0});
                    
                    }

                    // SellwareProduct product = new SellwareProduct(){
                    //     product=itemList,
                    //     pageSize= 100,
                    //     pageNumber= Int16.Parse(page),
                    //     totalResults = resultsCount.Count
                    // };
                    return StatusCode(StatusCodes.Status200OK, itemList); 



                }


            }


            if (abx.Equals("retrieve_orders"))
            {

                if (epochTimeStart==0 || epochTimeEnd==0) {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }

                var sql = "SELECT * FROM c WHERE c.type='tco/order' and c._ts>="+epochTimeStart +" and c._ts<= "+epochTimeEnd;

                
                orderList = (List<OrderFeed>)await _cosmosDbService.GetOrderFeedSellwareAsyncQuery(sql);

                return StatusCode(StatusCodes.Status200OK, orderList);
                
                //foreach order build response

            }


            return StatusCode(StatusCodes.Status400BadRequest);

        }

        private int GetSkuSellwareSales(List<ItemCommitment> commitmentList, string sku) {

            var skuCommitment = commitmentList.Find(i => i.sku==sku);
            if (skuCommitment==null) {
                return 0;
            } else {
                return skuCommitment.commited;
            }            

        }

        private async Task<List<ItemCommitment>> GetSellewareSales()
        {
            var queryString = "select * from c where c.type = 'tco/order' and c.status not in ('Canceled','Complete') ";

            var pendingSales = (List<SellwareOrder>) await _cosmosDbService.GetOrderSellwareAsyncQuery(queryString);

            List<ItemCommitment> commitmentItemList = new List<ItemCommitment>();

            foreach( var order in pendingSales ) {
                foreach( var item in order.orderItemList) {

                    var skuCommitment = commitmentItemList.Find(i => i.sku == item.sku);
                    if (skuCommitment==null) {
                        
                        ItemCommitment newCommitment = new ItemCommitment(){
                            sku=item.sku,
                            commited=item.qtyPurchased
                        };

                        commitmentItemList.Add(newCommitment);

                    } else {
                        skuCommitment.commited += item.qtyPurchased;
                    }

                }
            }
            return commitmentItemList;

        }

        static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }

}

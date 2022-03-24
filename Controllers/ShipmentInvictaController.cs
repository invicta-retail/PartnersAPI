using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using InvictaPartnersAPI.Models.Magento.Order;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using InvictaPartnersAPI.Models.Magento.Shipping;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers { 


    [Route("api/shipping")]
    [ApiController]

    public class ShipmentInvictaController : Controller {
        
        
        public IConfiguration _configuration;
        private readonly ILogger<ShipmentInvictaController> _logger;
        public ShipmentInvictaController(IConfiguration configuration, ILogger<ShipmentInvictaController> logger)
        {

            _configuration = configuration;
            _logger = logger;
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateShipment(int SupplierID, ShipmentRoot _ShipmentRoot )
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            //Ignoring Invicta Europe Orders
            if (_ShipmentRoot.orderNumber.Substring(0,3)=="450" && _ShipmentRoot.mageShipmentId==0 ) {
                _logger.LogInformation("InvictaEurope order");
                _logger.LogInformation("Tracking updated by InvictaPartnersWorker Job");
                return StatusCode(StatusCodes.Status200OK);
            }

            int mageShipmentId = 0;  
            try {
                //Get Magento Token
                _logger.LogInformation("Extracting magento token");
                var magentoTokenJson = JObject.Parse(await MagentoSupport.GetMagentoToken("invicta",_configuration));
                var token = magentoTokenJson["token"].ToString();
                _logger.LogInformation($"Token : {token}");

                //Get Magento Order
                _logger.LogInformation($"Getting order from Magento");
                OrderRoot magentoResponse = JsonConvert.DeserializeObject<OrderRoot>(await MagentoSupport.GetMagentoOrder("invicta",token, _ShipmentRoot.orderNumber,_configuration));
                string orderStatus = magentoResponse.items[0].status;
                _logger.LogInformation($"Order status : {orderStatus}");
                int orderId = magentoResponse.items[0].entity_id;
                _logger.LogInformation($"Order id : {orderId}");


                _logger.LogInformation("Replace Tracking with Fedex Cross World before posting to Magento if applicable");
                if (magentoResponse.items[0].extension_attributes.fdxcData != null) {
                    string fdxcbCarrier = "fedex";
                    var tracarray = magentoResponse.items[0].extension_attributes.fdxcData.tracking_link.Split('/');
                    string fdxcbTracking  = tracarray[tracarray.Length-1];
                    foreach(var dtl in _ShipmentRoot.details) {
                        dtl.trackingNumber=fdxcbTracking;
                        dtl.carrier=fdxcbCarrier;
                    }
                };

                _logger.LogInformation("Getting Magento Shipments");
                GetShippingRoot magentoShipmentResponse = JsonConvert.DeserializeObject<GetShippingRoot>(await MagentoSupport.GetMagentoShipments("invicta",token, _ShipmentRoot.orderNumber,_configuration)); 

                int existingShipmentId = 0;
                foreach (var shiprec in magentoShipmentResponse.items) {
                    foreach (var trac in shiprec.tracks) {
                        foreach (var voidtrac in _ShipmentRoot.voidTracking) {
                            if(voidtrac.Equals(trac.track_number)) {
                                existingShipmentId= trac.parent_id;
                                //void tracking found                                
                            }
                        }
                    }
                } 

                if (existingShipmentId==0 && orderStatus!="processing")  {
                    mageShipmentId = -200; //Shipment cannot be posted
                } else {

                    _logger.LogInformation("Post Tracking Update to Magento");
                    if (existingShipmentId>0) {

                        Entity _entity = new Entity(){
                            order_id =orderId,
                            parent_id =mageShipmentId,
                            track_number = _ShipmentRoot.details[0].trackingNumber,
                            title = _ShipmentRoot.details[0].carrier,
                            carrier_code=_ShipmentRoot.details[0].carrier
                        };
                        TrackUpdate tracUpdate = new TrackUpdate(){
                            entity= _entity    
                        };
                        await MagentoSupport.PostTrackingUpdate("invicta",token,tracUpdate,_configuration);
                        _logger.LogInformation("Post Tracking Updated successfully");
                        

                    } else {
                        _logger.LogInformation("Post New Shipment to Magento");
                        string source = "cw";
                        mageShipmentId= await MagentoSupport.PostMagentoShipment("invicta",token,source,orderId,magentoResponse,_ShipmentRoot,_configuration);
                        _logger.LogInformation("Magento Shipment Posted successfully");
                    }


                }

            } catch (Exception e) {
                _logger.LogError("Magento Exception: "+e.ToString());
                mageShipmentId = 0; 
            }


            _ShipmentRoot.mageShipmentId=mageShipmentId;

            var uri = _configuration.GetSection("invictasupplierurl").Value+"/upload/shipping?SupplierID="+SupplierID.ToString();
            _logger.LogInformation("uri:"+uri);
            Uri u = new Uri(uri);
            string payload = System.Text.Json.JsonSerializer.Serialize(_ShipmentRoot);
            _logger.LogInformation("payload: "+payload);
            HttpContent c = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var response = string.Empty;
            using (var client = new HttpClient())            
            {

                HttpResponseMessage result = await client.PostAsync(u, c);
                _logger.LogInformation("Result:"+result.Headers.ToString());
                _logger.LogInformation("Response Code:"+result.StatusCode);

                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                    return StatusCode(StatusCodes.Status201Created,payload);
              
                } else {
                    response = await result.Content.ReadAsStringAsync();
                }

            }
            
            return StatusCode(StatusCodes.Status400BadRequest,response);

        }

    }

}
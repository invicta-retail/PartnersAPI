using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers { 


    [Route("api/tco/shipping")]
    [ApiController]

    public class ShipmentTCOController : Controller {
        
        
        public IConfiguration _configuration;
        private readonly ILogger<ShipmentTCOController> _logger;
        public ShipmentTCOController(IConfiguration configuration, ILogger<ShipmentTCOController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateShipment(int SupplierID, ShipmentRoot _ShipmentRoot )
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            string uri = _configuration.GetSection("tcosupplierurl").Value+"/upload/tco/shipping?SupplierID="+SupplierID.ToString();
            _logger.LogInformation("uri:"+uri);
            Uri u = new Uri(uri);
            string payload = JsonSerializer.Serialize(_ShipmentRoot);
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
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using InvictaPartnersAPI.Interfaces;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers
{


    [Route("api/timestamp")]
    [ApiController]    public class TimeStampController : Controller
    {
        private readonly ICosmosDbService _cosmosDbService;
        public IConfiguration _configuration;
        private readonly ILogger<TimeStampController> _logger;

        public TimeStampController(ICosmosDbService cosmosDbService,IConfiguration configuration, ILogger<TimeStampController> logger)
        {
            _cosmosDbService = cosmosDbService;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<TimeStamp> SetTimeStamp(string id)
        {
            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            _logger.LogInformation($"Timestamp : {id}");
            TimeStamp _timestamp = await _cosmosDbService.SetTimeStampAsync(id);
            return _timestamp;
            
        }


    }
}
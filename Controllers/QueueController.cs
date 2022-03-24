using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using InvictaPartnersAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using RabbitMQ.Client;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace InvictaPartnersAPI.Controllers { 


    [Route("api/queue/forward")]
    [ApiController]

    public class QueueController : Controller {
        
        
        public IConfiguration _configuration;
        private readonly ILogger<QueueController> _logger;
        public QueueController(IConfiguration configuration, ILogger<QueueController> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost]
        public async Task<ActionResult> GetWebHook(string queue, SSWebHook _webHook )
        {

            var address = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            _logger.LogInformation($"Resquest by {address} IP");
            var section = _configuration.GetSection("RabbitMQ");
            _logger.LogInformation("Account: "+section.GetSection("Account").Value);

            var factory = new ConnectionFactory() { 
                HostName = section.GetSection("Host").Value,
                Port = Int32.Parse(section.GetSection("Port").Value),
                UserName = section.GetSection("User").Value,
                Password = section.GetSection("Password").Value,
                VirtualHost = "/",  
                Ssl = {                
                        ServerName =  section.GetSection("Host").Value,
                        Enabled = true
                    }    
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    string message = JsonSerializer.Serialize(_webHook);

                    var body = Encoding.UTF8.GetBytes(message);
                    
                    var properties = channel.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("x-delay", 7200000); //2 hrs

                    channel.BasicPublish(exchange: "",
                                        routingKey: queue,
                                        basicProperties: properties,
                                        body: body);
                }

        
            }
            await Task.Run(() => { });

            return StatusCode(StatusCodes.Status201Created);

        }

    }

}
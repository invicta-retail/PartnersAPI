using InvictaPartnersAPI.Exceptions;
using InvictaPartnersAPI.Interfaces;
using InvictaPartnersAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(IConfiguration configuration, ILogger<SupplierService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> PostInventory(List<InventoryEntry> items,int supplierId)
        {
            try
            {
                _logger.LogDebug($"Preparing supplier inventory: {supplierId}");
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                var modelJson = JsonSerializer.Serialize(items, options);
                _logger.LogDebug(modelJson);
                _logger.LogDebug("URI:" + _configuration.GetSection("tcouri").Value + "/upload/inventory?SupplierID=" + _configuration.GetSection("supplierId").Value);
                Uri u = new Uri(_configuration.GetSection("tcouri").Value + "/upload/inventory?SupplierID=" + supplierId);
                using (HttpClient httpClient = new HttpClient())
                {
                    _logger.LogDebug("Executing request");
                    HttpContent c = new StringContent(modelJson, System.Text.Encoding.UTF8, "application/json");
                    var result = await httpClient.PostAsync(u, c);
                    var response = await result.Content.ReadAsStringAsync();
                    _logger.LogDebug("Result:" + response);
                    if (result.IsSuccessStatusCode)
                    {
                        _logger.LogDebug("Inventory Update accepted");
                        return true;
                    }
                    else
                    {
                        _logger.LogDebug("Inventory Updated failed");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug("Inventory Updated failed");
                _logger.LogDebug("Exception : " + e.Message);
                throw new BusinessException("The service is not responding");
            }

        }

        public async Task<List<Excel>> GettingExcelOrders(int supplierId)
        {
            using(HttpClient client = new HttpClient())
            {
                _logger.LogDebug($"Preparing a db query to extract orders from the supplier: {supplierId}");
                Uri u = new Uri(_configuration.GetSection("tcosupplierurl").Value + "/api/Supplier/supplierId?supplierId=" + supplierId);
                var result = await client.GetAsync(u);
                var response = await result.Content.ReadAsStringAsync();
                _logger.LogDebug($"Response: {response}");
                if (result.IsSuccessStatusCode)
                {
                    _logger.LogDebug("Extraction completed");
                    var items = JsonSerializer.Deserialize<List<Excel>>(response);
                    return items;
                }
                else
                {
                    _logger.LogDebug("Request failed");
                    throw new BusinessException("The service is not responding");
                }
            }
        }
    }
}

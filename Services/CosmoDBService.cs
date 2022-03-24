using InvictaPartnersAPI.Exceptions;
using InvictaPartnersAPI.Interfaces;
using InvictaPartnersAPI.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaPartnersAPI
{

    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }


        public async Task<List<PriceEntry>> GetLatestPriceAsync(string sku,string type)
        {
            try
            {
                var queryString = "select * from c where c.type='" + type + "' and c.sku='" + sku + "' order by c.validDate desc ";
                var query = this._container.GetItemQueryIterator<PriceEntry>(new QueryDefinition(queryString));
                List<PriceEntry> results = new List<PriceEntry>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                return results;

            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            

        }

        public async Task<PriceEntry> GetPriceAsync(string id,string type)
        {

            try
            {
                ItemResponse<PriceEntry> response = await this._container.ReadItemAsync<PriceEntry>(id.Replace("/","-"), new PartitionKey(type));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task AddPriceAsync(PriceEntry entry,string type)
        {
           
            try
            {
                await this._container.CreateItemAsync<PriceEntry>(entry, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
        }

        public async Task UpdatePriceAsync(string id, PriceEntry entry)
        {
            try
            {
                await this._container.UpsertItemAsync<PriceEntry>(entry, new PartitionKey(entry.type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task AddItemAsync(Item item,string type)
        {
            try
            {
                await this._container.CreateItemAsync<Item>(item, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task DeleteItemAsync(string id,string type)
        {
            try
            {
                await this._container.DeleteItemAsync<Item>(id.Replace("/", "-"), new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<Item> GetItemAsync(string id,string type)
        {
            try
            {
                ItemResponse<Item> response = await this._container.ReadItemAsync<Item>(id.Replace("/","-"), new PartitionKey(type));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Item>> GetItemsAsync(string queryString)
        {
            try
            {
                var query = this._container.GetItemQueryIterator<Item>(new QueryDefinition(queryString));
                List<Item> results = new List<Item>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                return results;
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task UpdateItemAsync(string id, Item item)
        {
            try
            {
                await this._container.UpsertItemAsync<Item>(item, new PartitionKey(item.type));

            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task AddOrderAsync(Order order, string type)
        {
            try
            {
                await this._container.CreateItemAsync<Order>(order, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }

        }

        public async Task AddOrderSellwareAsync(SellwareOrder order, string type)
        {
            try
            {
                await this._container.CreateItemAsync<SellwareOrder>(order, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task DeleteOrderAsync(string id,string type)
        {
            try
            {
                await this._container.DeleteItemAsync<Order>(id, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<Order> GetOrderAsync(string id,string type)
        {
            try
            {
                ItemResponse<Order> response = await this._container.ReadItemAsync<Order>(id, new PartitionKey(type));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<SellwareOrder> GetOrderSellwareAsync(string id,string type)
        {
            try
            {
                ItemResponse<SellwareOrder> response = await this._container.ReadItemAsync<SellwareOrder>(id, new PartitionKey(type));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }
        
        public async Task<IEnumerable<Order>> GetOrderAsyncQuery(string queryString)
        {
            try
            {
                var query = this._container.GetItemQueryIterator<Order>(new QueryDefinition(queryString));
                List<Order> results = new List<Order>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                return results;
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }
        
        public async Task<IEnumerable<SellwareOrder>> GetOrderSellwareAsyncQuery(string queryString)
        {
            try
            {
                var query = this._container.GetItemQueryIterator<SellwareOrder>(new QueryDefinition(queryString));
                List<SellwareOrder> results = new List<SellwareOrder>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                return results;
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }        
        
        public async Task<IEnumerable<OrderFeed>> GetOrderFeedSellwareAsyncQuery(string queryString)
        {
            try
            {
                var query = this._container.GetItemQueryIterator<OrderFeed>(new QueryDefinition(queryString));
                List<OrderFeed> results = new List<OrderFeed>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                return results;
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }

        }

        public async Task UpdateOrderAsync(string id, Order order,string type)
        {
            try
            {
                await this._container.UpsertItemAsync<Order>(order, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }

        }

        public async Task UpdateOrderSellwareAsync(string id, SellwareOrder order,string type)
        {
            try
            {
                await this._container.UpsertItemAsync<SellwareOrder>(order, new PartitionKey(type));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<Sequence> GetSequenceAsync(string id)
        {
            try
            {
                ItemResponse<Sequence> response = await this._container.ReadItemAsync<Sequence>(id, new PartitionKey("sequence"));
                Sequence update = response;
                update.sequence += 1;
                await this._container.UpsertItemAsync<Sequence>(update, new PartitionKey("sequence"));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<TimeStamp> GetTimeStampAsync(string id) {
            try
            {
                ItemResponse<TimeStamp> response = await this._container.ReadItemAsync<TimeStamp>(id, new PartitionKey("timestamp"));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<TimeStamp> SetTimeStampAsync(string id) {
            try
            {  
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                TimeStamp timestamp = new TimeStamp() {
                    id=id,
                    type="timestamp",
                    timestamp=unixTimestamp
                };
                await this._container.UpsertItemAsync<TimeStamp>(timestamp, new PartitionKey("timestamp"));

                ItemResponse<TimeStamp> response = await this._container.ReadItemAsync<TimeStamp>(id, new PartitionKey("timestamp"));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Models.User>> GetUserAsyncQuery(string query)
        {
            try
            {
                var _query = this._container.GetItemQueryIterator<Models.User>(new QueryDefinition(query));
                List<Models.User> results = new List<Models.User>();
                while (_query.HasMoreResults)
                {
                    var response = await _query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
                return results;
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<Models.User> GetUserAsync(string id)
        {
            if (id ==null) return null;
            
            try
            {
                ItemResponse<Models.User> response = await this._container.ReadItemAsync<Models.User>(id, new PartitionKey("user"));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<Models.User> AddUserAsync(Models.User user)
        {
            try
            {
                ItemResponse<Models.User> response = await this._container.CreateItemAsync<Models.User>(user, new PartitionKey("user"));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task UpdateUserAsync(string id, Models.User user)
        {
            try
            {
                await this._container.UpsertItemAsync<Models.User>(user, new PartitionKey("user"));
            }
            catch (Exception e)
            {
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

    }
}

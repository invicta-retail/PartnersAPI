using InvictaPartnersAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Interfaces
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Item>> GetItemsAsync(string query);
        Task<Item> GetItemAsync(string id, string type);
        Task AddItemAsync(Item item, string type);
        Task UpdateItemAsync(string id, Item item);
        Task DeleteItemAsync(string id, string type);
        Task<IEnumerable<Order>> GetOrderAsyncQuery(string query);
        Task<IEnumerable<SellwareOrder>> GetOrderSellwareAsyncQuery(string queryString);
        Task<IEnumerable<OrderFeed>> GetOrderFeedSellwareAsyncQuery(string queryString);
        Task<Order> GetOrderAsync(string id, string type);
        Task<SellwareOrder> GetOrderSellwareAsync(string id, string type);
        Task AddOrderAsync(Order order, string type);
        Task AddOrderSellwareAsync(SellwareOrder order, string type);
        Task UpdateOrderAsync(string id, Order order, string type);
        Task UpdateOrderSellwareAsync(string id, SellwareOrder order, string type);
        Task DeleteOrderAsync(string id, string type);
        Task<Sequence> GetSequenceAsync(string id);
        Task<IEnumerable<Models.User>> GetUserAsyncQuery(string query);
        Task<Models.User> GetUserAsync(string id);
        Task<Models.User> AddUserAsync(Models.User user);
        Task UpdateUserAsync(string id, Models.User user);
        Task<TimeStamp> GetTimeStampAsync(string id);
        Task<TimeStamp> SetTimeStampAsync(string id);
        Task<List<PriceEntry>> GetLatestPriceAsync(string sku, string type);
        Task<PriceEntry> GetPriceAsync(string id, string type);
        Task AddPriceAsync(PriceEntry item, string type);
        Task UpdatePriceAsync(string id, PriceEntry entry);
    }
}

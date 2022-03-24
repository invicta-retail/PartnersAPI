using InvictaPartnersAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvictaPartnersAPI.Interfaces
{
    public interface ISupplierService
    {
        Task<bool> PostInventory(List<InventoryEntry> items, int supplierId);
        Task<List<Excel>> GettingExcelOrders(int supplierId);
    }
}

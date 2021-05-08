using DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IItemService
    {
        Task<IEnumerable<ItemModel>> GetItems();
        Task<ItemResponse> UpsertItem(ItemModel itemModel, string userId);
    }
}

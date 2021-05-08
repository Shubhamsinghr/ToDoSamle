using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        IEnumerable<Item> GetAllItem();
        Task<ItemResponse> UpsertItem(Item item, string userId);
    }
}

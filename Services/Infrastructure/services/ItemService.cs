using AutoMapper;
using DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;
        public ItemService(IItemRepository itemRepository, IMapper mapper)
        {
            this._itemRepository = itemRepository;
            this._mapper = mapper;
        }
        public Task<IEnumerable<ItemModel>> GetItems()
        {
            var items = _mapper.Map<IEnumerable<Item>, IEnumerable<ItemModel>>(_itemRepository.GetAllItem());
            return Task.FromResult(items);
        }

        public Task<ItemResponse> UpsertItem(ItemModel itemModel, string userId)
        {
            var item = _mapper.Map<ItemModel, Item>(itemModel);
            return _itemRepository.UpsertItem(item, userId);
        }
    }
}

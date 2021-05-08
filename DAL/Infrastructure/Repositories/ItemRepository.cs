using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        ToDoDbContext _context;
        public ItemRepository(ToDoDbContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<Item> GetAllItem()
        {
            return _context.Item;
        }

        public async Task<ItemResponse> UpsertItem(Item model, string userId)
        {
            AuditLog newLog = new AuditLog();
            ItemResponse res = new ItemResponse();
            string newValues = "NewTitle = " + model.Title + ", NewDescription = " + model.Description;
            if (model.Id != 0)
            {
                var entity = _context.Item.FirstOrDefault(item => item.Id == model.Id);
                if (entity != null)
                {
                    entity.Title = model.Title;
                    entity.Description = model.Description;
                    entity.CreatedAt = entity.CreatedAt;
                    entity.ModifedOn = DateTime.UtcNow;
                    entity.ModifedBy = userId;
                    await _context.SaveChangesAsync();
                    string oldValues = "OldTitle = " + entity.Title + ", OldDescription = " + entity.Description;
                    newLog = new AuditLog(userId, "Edit Item", entity.Id, oldValues, newValues);
                }
            }
            else
            {
                model.AccountId = userId;
                model.CreatedBy = userId;
                model.CreatedAt = DateTime.UtcNow;
                await _context.Item.AddAsync(model);
                await _context.SaveChangesAsync();
                newLog = new AuditLog(userId, "Add New Item", model.Id, "", newValues);
            }
            res.Id = model.Id;
            res.AuditLog = newLog;
            return res;
        }

        private ToDoDbContext MyMusicDbContext
        {
            get { return Context as ToDoDbContext; }
        }
    }
}

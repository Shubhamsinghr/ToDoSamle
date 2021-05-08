using System.Threading.Tasks;

namespace DAL
{
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        ToDoDbContext _context;
        public AuditLogRepository(ToDoDbContext context)
            : base(context)
        {
            _context = context;
        }
        public Task<int> UpdateLog(AuditLog log)
        {
            _context.AuditLog.AddAsync(log);
            _context.SaveChangesAsync();
            return Task.FromResult(log.Id);
        }
    }
}

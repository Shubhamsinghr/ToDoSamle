using System.Threading.Tasks;

namespace DAL
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
        Task<int> UpdateLog(AuditLog log);
    }
}

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.DB.Models
{
    public interface IDbRepository
    {
        Task CreateTables();
        Task<int> NonQueryFromSql(string sql, params object[] sqliteParams);
        Task SaveChangesAsync();
        DbSet<DbMessage> Messages { get; set; }
    }
}
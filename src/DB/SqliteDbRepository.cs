using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication.DB.Models;

namespace WebApplication.DB
{
    public class SqliteDbRepository : DbContext, IDbRepository
    {
        private const string DbName = "messages.dat";

        public DbSet<DbMessage> Messages { get; set; }

        public SqliteDbRepository(DbContextOptions<SqliteDbRepository> options) : base(options)
        {}
        
        public async Task CreateTables()
        {
            foreach (var dbSchema in DbTables.Schemas)
            {
                await CreateTable(dbSchema.Key, dbSchema.Value);
            }
        }

        async Task CreateTable(string tableName, string schema)
        {
            await NonQueryFromSql($"Create Table if not exists \"{tableName}\"" + $" ({schema});");
        }

        public async Task<int> NonQueryFromSql(string sql, params object[] sqliteParams)
        {
            return await Database.ExecuteSqlRawAsync(sql, sqliteParams);
        }

        public Task SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName)}");
        }
    }
}
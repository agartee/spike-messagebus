using Microsoft.EntityFrameworkCore;
using Spike.Common.Services;
using Spike.SqlServer.Models;

namespace Spike.SqlServer
{
    public class SpikeDbContext : DbContext, IUnitOfWork
    {
        public const string SchemaName = "Spike";

        public SpikeDbContext(DbContextOptions<SpikeDbContext> options) : base(options) { }

        public DbSet<PersonData> People { get; set; }
        public DbSet<MessageData> MessageOutbox { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);
        }
    }
}

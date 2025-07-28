using Microsoft.EntityFrameworkCore;
using Spike.Common.Services;
using Spike.Messaging.SqlServer.Models;
using Spike.Messaging.SqlServer.Services;
using Spike.SqlServer.Models;

namespace Spike.SqlServer
{
    public class SpikeDbContext : DbContext, IUnitOfWork, IMessageOutbox
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

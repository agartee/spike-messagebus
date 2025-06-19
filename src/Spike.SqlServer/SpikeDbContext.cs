using Microsoft.EntityFrameworkCore;
using Spike.SqlServer.Models;

namespace Spike.SqlServer
{
    public class SpikeDbContext : DbContext
    {
        public const string SCHEMA_NAME = "Spike";

        public SpikeDbContext(DbContextOptions<SpikeDbContext> options) : base(options) { }

        public DbSet<PersonData> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);
        }
    }
}

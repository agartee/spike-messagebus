using Microsoft.EntityFrameworkCore;
using Spike.Common.Services;
using Spike.Domain.Models;
using Spike.Messaging.SqlServer.Models;
using Spike.Messaging.SqlServer.Services;
using Spike.SqlServer.Converters;
using Spike.SqlServer.Models;

namespace Spike.SqlServer
{
    public class SpikeDbContext : DbContext, IUnitOfWork, IMessageOutbox
    {
        public const string SchemaName = "Spike";

        public SpikeDbContext(DbContextOptions<SpikeDbContext> options) : base(options) { }

        public DbSet<PersonData> People { get; set; }
        public DbSet<PlaceData> Places { get; set; }
        public DbSet<MessageData> MessageOutbox { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);

            var personIdConverter = new StronglyTypedIdConverter<PersonId>(
                guid => new PersonId(guid),
                id => id.Value);

            var placeIdConverter = new StronglyTypedIdConverter<PlaceId>(
                guid => new PlaceId(guid),
                id => id.Value);

            modelBuilder.Entity<Person>()
                .Property(p => p.Id)
                .HasConversion(personIdConverter);

            modelBuilder.Entity<Place>()
                .Property(p => p.Id)
                .HasConversion(placeIdConverter);
        }
    }
}

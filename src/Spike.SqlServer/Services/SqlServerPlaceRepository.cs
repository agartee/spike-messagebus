using Spike.Domain.Models;
using Spike.Domain.Services;
using Spike.Messaging.Services;
using Spike.SqlServer.Models;

namespace Spike.SqlServer.Services
{
    public class SqlServerPlaceRepository : IPlaceRepository
    {
        private readonly SpikeDbContext dbContext;

        // note: could be in base class rather than injected service for this paradigm (e.g., AggregateRepository)
        private readonly IMessageOutboxWriter messageOutboxWriter;

        public SqlServerPlaceRepository(SpikeDbContext dbContext, IMessageOutboxWriter messageOutboxWriter)
        {
            this.dbContext = dbContext;
            this.messageOutboxWriter = messageOutboxWriter;
        }
        public async Task SavePlace(Place place, CancellationToken cancellationToken)
        {
            // note: should be extracted to an extension method, but included here for discussion clarity
            var data = new PlaceData
            {
                Id = place.Id,
                Name = place.Name
            };

            dbContext.Places.Add(data);

            foreach (var domainEvent in place.DomainEvents)
                messageOutboxWriter.AddMessage(domainEvent, place.Id);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

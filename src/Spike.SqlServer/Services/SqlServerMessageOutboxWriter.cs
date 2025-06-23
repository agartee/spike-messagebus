using Spike.Domain.Services;
using Spike.SqlServer.Models;
using System.Text.Json;

namespace Spike.SqlServer.Services
{
    public class SqlServerMessageOutboxWriter : IMessageOutboxWriter
    {
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private readonly SpikeDbContext dbContext;

        public SqlServerMessageOutboxWriter(SpikeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void AddMessage(object domainEvent)
        {
            var json = JsonSerializer.Serialize(domainEvent, serializerOptions);

            dbContext.MessageOutbox.Add(new MessageData
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                CommitSequence = 0,
                Body = json,
                TypeName = domainEvent.GetType().FullName
            });
        }
    }
}

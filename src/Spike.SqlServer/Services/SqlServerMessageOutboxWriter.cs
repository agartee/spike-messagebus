using Spike.Common.Models;
using Spike.Common.Services;
using Spike.SqlServer.Models;
using Spike.WebApp.Services;
using System.Text.Json;

namespace Spike.SqlServer.Services
{
    public class SqlServerMessageOutboxWriter : IMessageOutboxWriter
    {
        private readonly SpikeDbContext dbContext;
        private readonly SqlServerMessageOutboxOptions options;

        public SqlServerMessageOutboxWriter(SpikeDbContext dbContext, SqlServerMessageOutboxOptions options)
        {
            this.dbContext = dbContext;
            this.options = options;
        }

        public void AddMessage(object domainEvent, IStronglyTypedId aggretateRootId)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            var json = JsonSerializer.Serialize(domainEvent, options.JsonSerializerOptions);

            dbContext.MessageOutbox.Add(new MessageData
            {
                Id = Guid.NewGuid(),
                CorrelationId = aggretateRootId.Value,
                Created = DateTime.UtcNow,
                CommitSequence = 0,
                Body = json,
                TypeName = domainEvent.GetType().AssemblyQualifiedName!
            });
        }
    }
}

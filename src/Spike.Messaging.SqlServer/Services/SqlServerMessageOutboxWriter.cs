using Microsoft.EntityFrameworkCore;
using Spike.Common.Models;
using Spike.Messaging.Services;
using Spike.Messaging.SqlServer.Models;
using Spike.Messaging.SqlServer.Services;
using System.Text.Json;

namespace Spike.SqlServer.Services
{
    public class SqlServerMessageOutboxWriter<TDbContext> : IMessageOutboxWriter
        where TDbContext : DbContext, IMessageOutbox
    {
        private readonly TDbContext dbContext;
        private readonly SqlServerMessageOutboxOptions options;

        public SqlServerMessageOutboxWriter(TDbContext dbContext, SqlServerMessageOutboxOptions options)
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

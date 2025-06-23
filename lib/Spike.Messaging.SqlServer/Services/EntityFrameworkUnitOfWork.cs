using Microsoft.EntityFrameworkCore;
using Spike.Messaging.Services;

namespace Spike.Messaging.SqlServer.Services
{
    public class EntityFrameworkUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext dbContext;
        private bool committed;

        public EntityFrameworkUnitOfWork(TDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Commit(CancellationToken cancellationToken = default)
        {
            if (committed) 
                return;

            await dbContext.SaveChangesAsync(cancellationToken);
            committed = true;
        }

        public ValueTask DisposeAsync()
        {
            if (!committed)
                throw new InvalidOperationException($"{GetType()} was disposed without being committed. Call CommitAsync() before disposing.");

            return ValueTask.CompletedTask;
        }
    }
}

using Spike.Messaging.Models;

namespace Spike.Domain.Services
{
    public interface IMessageOutboxReader
    {
        Task<IEnumerable<DomainMessageInfo>> GetNextBatch(int batchSize, int retryAfterMins, int maxRetries);
        Task ReportSuccess(Guid domainMessageId);
        Task ReportFailure(Guid domainMessage);
    }
}

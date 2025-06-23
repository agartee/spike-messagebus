using Spike.Messaging.Models;

namespace Spike.Messaging.Services
{
    public interface IMessageOutboxReader
    {
        Task<IEnumerable<DomainMessageInfo>> GetNextBatch(int batchSize, int retryAfterMins, int maxRetries);
        Task ReportSuccess(DomainMessageId domainMessageId);
        Task ReportFailure(DomainMessageId domainMessage);
    }
}

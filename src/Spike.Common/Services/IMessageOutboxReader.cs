using Spike.Common.Models;

namespace Spike.Common.Services
{
    public interface IMessageOutboxReader
    {
        Task<IEnumerable<DomainMessageInfo>> GetNextBatch(int batchSize, int retryAfterMins, int maxRetries);
        Task ReportSuccess(Guid domainMessageId);
        Task ReportFailure(Guid domainMessage);
    }
}

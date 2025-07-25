using Spike.Common.Models;

namespace Spike.Common.Services
{
    public interface IMessageOutboxReader
    {
        Task<IEnumerable<DomainMessageInfo>> GetNextBatch();
        Task ReportSuccess(Guid domainMessageId);
        Task ReportFailure(Guid domainMessage);
    }
}

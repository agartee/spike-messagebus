using Spike.Common.Models;
using Spike.Messaging.Models;

namespace Spike.Messaging.Services
{
    public interface IMessageOutboxReader
    {
        Task<IEnumerable<DomainMessageInfo>> GetNextBatch();
        Task ReportSuccess(Guid domainMessageId);
        Task ReportFailure(Guid domainMessage);
    }
}

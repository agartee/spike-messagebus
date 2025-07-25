using Spike.Common.Models;

namespace Spike.Common.Services
{
    public interface IMessageOutboxWriter
    {
        void AddMessage(object domainEvent, IStronglyTypedId aggretateId);
    }
}

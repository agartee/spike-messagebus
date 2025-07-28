using Spike.Common.Models;

namespace Spike.Messaging.Services
{
    public interface IMessageOutboxWriter
    {
        void AddMessage(object domainEvent, IStronglyTypedId aggretateId);
    }
}

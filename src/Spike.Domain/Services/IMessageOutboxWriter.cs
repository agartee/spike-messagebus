namespace Spike.Domain.Services
{
    public interface IMessageOutboxWriter
    {
        void AddMessage(object domainEvent);
    }
}

namespace Spike.Messaging.Services
{
    public interface IMessageOutboxWriter
    {
        Task EnqueueMessage(object message, CancellationToken cancellationToken);
    }
}

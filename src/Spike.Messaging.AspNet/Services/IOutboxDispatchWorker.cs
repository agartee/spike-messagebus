namespace Spike.Messaging.AspNet.Services
{
    public interface IOutboxDispatchWorker
    {
        Task DispatchPendingMessages(CancellationToken cancellationToken);
    }
}

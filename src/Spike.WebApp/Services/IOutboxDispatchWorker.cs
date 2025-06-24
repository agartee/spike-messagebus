namespace Spike.WebApp.Services
{
    public interface IOutboxDispatchWorker
    {
        Task DispatchPendingMessages(CancellationToken cancellationToken);
    }
}

namespace Spike.WebApp.Services
{
    public class OutboxDispatcherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxDispatcherService> _logger;

        public OutboxDispatcherService(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcherService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Outbox Dispatcher running.");
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var worker = scope.ServiceProvider.GetRequiredService<IOutboxDispatchWorker>();

                    await worker.DispatchPendingMessages(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in outbox dispatcher.");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}

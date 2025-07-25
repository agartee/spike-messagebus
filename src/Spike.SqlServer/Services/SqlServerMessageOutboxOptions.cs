namespace Spike.WebApp.Services
{
    public record SqlServerMessageOutboxOptions
    {
        public int BatchSize { get; init; } = 10;
        public int RetryAfterSeconds { get; init; } = 60;
        public int MaxRetries { get; init; } = 3;
    }
}

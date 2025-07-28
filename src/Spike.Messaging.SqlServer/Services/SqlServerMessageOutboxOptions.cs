using System.Text.Json;

namespace Spike.Messaging.SqlServer.Services
{
    public record SqlServerMessageOutboxOptions
    {
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public required string SchemaName { get; init; }
        public int BatchSize { get; init; } = 10;
        public int RetryAfterSeconds { get; init; } = 60;
        public int MaxRetries { get; init; } = 3;
        public JsonSerializerOptions JsonSerializerOptions => serializerOptions;
    }
}

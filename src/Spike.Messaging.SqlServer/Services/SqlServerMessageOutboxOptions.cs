using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spike.Messaging.SqlServer.Services
{
    public class SqlServerMessageOutboxOptions
    {
        public string SchemaName { get; set; } = "dbo";
        public int BatchSize { get; set; } = 10;
        public int RetryAfterSeconds { get; set; } = 60;
        public int MaxRetries { get; set; } = 3;
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
}

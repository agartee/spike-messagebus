namespace Spike.Messaging.Models
{
    public record DomainMessageInfo
    {
        public required Guid Id { get; init; }
        public string? TypeName { get; init; }
        public required string Body { get; init; }
        public required DateTime Created { get; init; }
        public required int CommitSequence { get; init; }
        public required int SendAttemptCount { get; init; }
        public DateTime? LastSendAttempt { get; init; }
    }
}

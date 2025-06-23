namespace Spike.Messaging.Models
{
    public record DomainMessageInfo
    {
        public required Guid Id { get; init; }
        public required string Json { get; init; }
        public required DateTime Created { get; init; }
        public required int CommitSequence { get; init; }
        public int SendAttemptCount { get; init; }
        public DateTime? LastSendAttempt { get; init; }
    }
}

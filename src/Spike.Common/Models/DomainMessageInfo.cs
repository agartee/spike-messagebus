namespace Spike.Common.Models
{
    public record DomainMessageInfo
    {
        public required DomainMessageId Id { get; init; }
        public required CorrelationId CorrelationId { get; init; }
        public required string TypeName { get; init; }
        public required string Body { get; init; }
        public required DateTime Created { get; init; }
        public required int CommitSequence { get; init; }
        public required MessageStatus Status { get; init; }
        public required int SendAttemptCount { get; init; }
        public DateTime? LastDequeuedAt { get; init; }
    }
}

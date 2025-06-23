namespace Spike.Messaging.Models
{
    public record struct DomainMessageId
    {
        public Guid Value { get; private set; }

        public DomainMessageId(Guid value)
        {
            Value = value;
        }
    }
}

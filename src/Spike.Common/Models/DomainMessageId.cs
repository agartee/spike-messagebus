namespace Spike.Common.Models
{
    public readonly record struct DomainMessageId(Guid Value) : IStronglyTypedId
    {
        public static DomainMessageId New() => new(Guid.NewGuid());
        public static DomainMessageId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(DomainMessageId id) => id.Value;
        public static implicit operator DomainMessageId(Guid value) => new(value);
    }
}

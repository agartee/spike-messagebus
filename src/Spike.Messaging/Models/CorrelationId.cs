using Spike.Common.Models;

namespace Spike.Messaging.Models
{
    public readonly record struct CorrelationId(Guid Value) : IStronglyTypedId
    {
        public static CorrelationId New() => new(Guid.NewGuid());
        public static CorrelationId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(CorrelationId id) => id.Value;
        public static implicit operator CorrelationId(Guid value) => new(value);
    }
}

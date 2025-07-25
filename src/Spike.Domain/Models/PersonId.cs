using Spike.Common.Models;

namespace Spike.Domain.Models
{
    public readonly record struct PersonId(Guid Value) : IStronglyTypedId
    {
        public static PersonId New() => new(Guid.NewGuid());
        public static PersonId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(PersonId id) => id.Value;
        public static implicit operator PersonId(Guid value) => new(value);
    }
}

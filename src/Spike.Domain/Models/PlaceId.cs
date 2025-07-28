using Spike.Common.Models;

namespace Spike.Domain.Models
{
    public readonly record struct PlaceId(Guid Value) : IStronglyTypedId
    {
        public static PlaceId New() => new(Guid.NewGuid());
        public static PlaceId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(PlaceId id) => id.Value;
        public static implicit operator PlaceId(Guid value) => new(value);
    }
}

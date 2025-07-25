using Spike.Common.Models;

namespace Spike.Domain.Models
{
    public record Person : IAggregateRoot<PersonId>
    {
        public PersonId Id { get; init; }
        public required string Name { get; set; }

        public object GetGenericId() => Id;
    }
}

using Spike.Common.Models;

namespace Spike.Domain.Models
{
    public record Person : IAggregateRoot<PersonId>
    {
        public required PersonId Id { get; init; }
        public required string Name { get; set; }

        public static Person New(string name) => new Person
        {
            Id = PersonId.New(),
            Name = name
        };
    }
}

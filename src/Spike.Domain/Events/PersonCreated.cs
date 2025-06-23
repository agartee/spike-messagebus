namespace Spike.Domain.Events
{
    public class PersonCreated
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }
}

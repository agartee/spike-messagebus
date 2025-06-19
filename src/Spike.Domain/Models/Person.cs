namespace Spike.Domain.Models
{
    public record Person
    {
        public Guid Id { get; init; }
        public required string Name { get; set; }
    }
}

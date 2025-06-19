namespace Spike.SqlServer.Models
{

    public record PersonData
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}

using Spike.Domain.Models;

namespace Spike.SqlServer.Models
{
    public record PlaceData
    {
        public PlaceId Id { get; set; }
        public required string Name { get; set; }
    }
}

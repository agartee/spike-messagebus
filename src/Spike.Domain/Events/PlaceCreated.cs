using Spike.Domain.Models;

namespace Spike.Domain.Events
{
    public class PlaceCreated
    {
        public required PlaceId Id { get; init; }
        public required string Name { get; init; }
    }
}

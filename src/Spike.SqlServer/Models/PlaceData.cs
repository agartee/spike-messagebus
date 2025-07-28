using Spike.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Spike.SqlServer.Models
{
    public record PlaceData
    {
        public PlaceId Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }
    }
}

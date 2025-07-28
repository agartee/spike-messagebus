using Spike.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Spike.SqlServer.Models
{

    public record PersonData
    {
        public PersonId Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }
    }
}

using Spike.Common.Models;
using Spike.Domain.Events;

namespace Spike.Domain.Models
{
    public record Place : IAggregateRoot<PlaceId>
    {
        private List<object> domainEvents = [];

        // private constructor to enforce use of factory method
        private Place() { }

        public required PlaceId Id { get; init; }
        public required string Name { get; set; }
        
        // todo: move to base class if this is the desired pattern
        public IEnumerable<object> DomainEvents => domainEvents.AsReadOnly();
        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }

        public static Place New(string name) {
            var place = new Place
            {
                Id = PlaceId.New(),
                Name = name
            };

            place.domainEvents.Add(new PlaceCreated
            {
                Id = place.Id,
                Name = place.Name
            });

            return place;
        }
    }
}

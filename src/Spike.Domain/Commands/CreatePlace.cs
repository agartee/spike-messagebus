using Spike.Domain.Models;
using Spike.Domain.Services;

namespace Spike.Domain.Commands
{
    public class CreatePlace
    {
        public required string Name { get; init; }
    }

    public class CreatePlaceHandler
    {
        public readonly IPlaceRepository placeRepository;

        public CreatePlaceHandler(IPlaceRepository placeRepository)
        {
            this.placeRepository = placeRepository;
        }

        public async Task Handle(CreatePlace command, CancellationToken cancellationToken)
        {
            var place = Place.New(command.Name);

            await placeRepository.SavePlace(place, cancellationToken);
        }
    }
}

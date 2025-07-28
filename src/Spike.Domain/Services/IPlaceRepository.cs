using Spike.Domain.Models;

namespace Spike.Domain.Services
{
    public interface IPlaceRepository
    {
        Task SavePlace(Place place, CancellationToken cancellationToken);
    }
}

using Spike.Domain.Models;

namespace Spike.Domain.Services
{
    public interface IPersonRepository
    {
        Task CreatePerson(Person person, CancellationToken cancellationToken);
    }
}

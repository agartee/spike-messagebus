using Spike.Domain.Models;

namespace Spike.Domain.Services
{
    public interface IPersonRepository
    {
        void AddPerson(Person person);
    }
}

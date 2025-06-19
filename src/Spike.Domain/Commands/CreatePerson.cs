using Spike.Domain.Models;
using Spike.Domain.Services;

namespace Spike.Domain.Commands
{
    public record CreatePerson
    {
        public required string Name { get; init; }
    }

    public class CreatePersonHandler
    {
        private readonly IPersonRepository personRepository;

        public CreatePersonHandler(IPersonRepository personRepository)
        {
            this.personRepository = personRepository;
        }

        public async Task Handle(CreatePerson command, CancellationToken cancellationToken)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Name = command.Name
            };

            await personRepository.CreatePerson(person, cancellationToken);
        }
    }
}

using Spike.Domain.Events;
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
        private readonly IMessageOutboxWriter messageOutboxWriter;
        private readonly IUnitOfWork unitOfWork;

        public CreatePersonHandler(IPersonRepository personRepository, IMessageOutboxWriter messageOutboxWriter, IUnitOfWork unitOfWork)
        {
            this.personRepository = personRepository;
            this.messageOutboxWriter = messageOutboxWriter;
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(CreatePerson command, CancellationToken cancellationToken)
        {
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Name = command.Name
            };

            personRepository.AddPerson(person);

            var domainEvent = new PersonCreated
            {
                Id = person.Id,
                Name = person.Name
            };

            messageOutboxWriter.AddMessage(domainEvent);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

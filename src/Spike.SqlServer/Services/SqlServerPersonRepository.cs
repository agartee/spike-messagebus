using Spike.Domain.Models;
using Spike.Domain.Services;
using Spike.SqlServer.Models;

namespace Spike.SqlServer.Services
{
    public class SqlServerPersonRepository : IPersonRepository
    {
        private readonly SpikeDbContext dbContext;

        public SqlServerPersonRepository(SpikeDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreatePerson(Person person, CancellationToken cancellationToken)
        {
            var data = new PersonData
            {
                Id = person.Id,
                Name = person.Name
            };

            dbContext.People.Add(data);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

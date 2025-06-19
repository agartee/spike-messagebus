using Spike.Domain.Commands;

namespace Spike.WebApp.Endpoints
{
    public static class CreatePersonMapping
    {
        public static IEndpointRouteBuilder MapCreatePerson(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/person", (
                CreatePerson command,
                CreatePersonHandler handler,
                CancellationToken cancellationToken) =>
                {
                    return handler.Handle(command, cancellationToken);
                })
                .WithName("CreatePerson")
                .WithSummary("Creates a new Person");

            return routeBuilder;
        }
    }
}

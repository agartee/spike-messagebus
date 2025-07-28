using Spike.Domain.Commands;

namespace Spike.WebApp.Endpoints
{
    public static class CreatePlaceEndpoint
    {
        public static IEndpointRouteBuilder MapCreatePlace(this IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/place", (
                CreatePlace command,
                CreatePlaceHandler handler,
                CancellationToken cancellationToken) =>
            {
                return handler.Handle(command, cancellationToken);
            })
                .WithName("CreatePlace")
                .WithSummary("Creates a new Place");

            return routeBuilder;
        }
    }
}

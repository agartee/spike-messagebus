using Spike.Common.Services;
using Spike.Domain.Models;
using Spike.Messaging.AspNet.DependencyInjection;
using Spike.Messaging.SqlServer.DependencyInjection;
using Spike.SqlServer;
using Spike.WebApp.DependencyInjection;
using Spike.WebApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDomainServices(builder.Configuration);

builder.Services
    .AddAspNetMessageDispatching(builder.Configuration)
    .AddSqlServerMessageOutbox<SpikeDbContext>(options =>
    {
        options.SchemaName = SpikeDbContext.SchemaName;
    }, scanAssemblyForIdTypes: typeof(PersonId).Assembly);

// don't love this
builder.Services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<SpikeDbContext>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCreatePerson();

app.Run();

using Spike.Messaging.AspNet.DependencyInjection;
using Spike.Messaging.SqlServer.DependencyInjection;
using Spike.SqlServer;
using Spike.WebApp.DependencyInjection;
using Spike.WebApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDomainServices(builder.Configuration);

builder.Services
    .AddSqlServerMessageOutbox<SpikeDbContext>()
    .AddAspNetMessageDispatching(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCreatePerson();

app.Run();

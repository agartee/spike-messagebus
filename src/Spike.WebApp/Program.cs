using Spike.Common.Services;
using Spike.Messaging.AspNet.DependencyInjection;
using Spike.Messaging.SqlServer.DependencyInjection;
using Spike.Messaging.SqlServer.Services;
using Spike.SqlServer;
using Spike.WebApp.DependencyInjection;
using Spike.WebApp.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDomainServices(builder.Configuration);

builder.Services.AddSqlServerMessageOutbox<SpikeDbContext>(new SqlServerMessageOutboxOptions
{
    SchemaName = SpikeDbContext.SchemaName
});
builder.Services.AddAspNetMessageDispatching(builder.Configuration);

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

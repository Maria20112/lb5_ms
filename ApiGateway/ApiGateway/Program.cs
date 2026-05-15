using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("configuration.json", optional: false, reloadOnChange: true).Build();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddOcelot();

var app = builder.Build();

await app.UseOcelot();

app.Run();

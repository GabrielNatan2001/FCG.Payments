using FCG.Payments.Application;
using FCG.Payments.Infrastructure;
using FCG.Payments.Infrastructure.Data;
using FCG.Payments.Worker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecksInfrastructure(builder.Configuration);

builder.Services.Configure<OrderPlacedWorkerConfig>(
    builder.Configuration.GetSection("Workers:OrderPlaced"));
builder.Services.AddHostedService<OrderPlacedWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.MapHealthChecks("/health");
app.Run();

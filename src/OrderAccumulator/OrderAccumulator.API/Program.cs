using Microsoft.EntityFrameworkCore;
using OrderAccumulator.Application.UseCases;
using OrderAccumulator.Domain.Interfaces;
using OrderAccumulator.Infrastructure.Persistence;
using OrderAccumulator.API.Fix;
using OpenTelemetry.Shared;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddSharedOpenTelemetry();

builder.Services.AddDbContext<OrderAccumulatorDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});
builder.Services.AddScoped<IExposureRepository, ExposureRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ProcessOrderUseCase>();
builder.Services.AddScoped<ExposureInitializer>();

builder.Services.AddSingleton<FixAcceptor>();
builder.Services.AddHostedService<FixAcceptorHostedService>();

var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetRequiredService<ExposureInitializer>();
        await initializer.InitializeAsync();
    }

app.MapPrometheusScrapingEndpoint();

app.Run();

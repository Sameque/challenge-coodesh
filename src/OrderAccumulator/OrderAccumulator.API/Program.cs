using Microsoft.EntityFrameworkCore;
using OrderAccumulator.Application.UseCases;
using OrderAccumulator.Domain.Interfaces;
using OrderAccumulator.Infrastructure.Persistence;
using OrderAccumulator.API.Fix;
using QuickFix;
using QuickFix.Store;
using QuickFix.Logger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderAccumulatorDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

// Persistence
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<ProcessOrderUseCase>();
builder.Services.AddScoped<GetSymbolsUseCase>();
builder.Services.AddScoped<GetExposureUseCase>();

builder.Services.AddSingleton<FixAcceptor>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderAccumulatorDbContext>();
    db.Database.EnsureCreated();
    if (!db.Symbols.Any())
    {
        db.Symbols.AddRange(
            new OrderAccumulator.Domain.Entities.Symbol { Ticker = "PETR4", Description = "Petrobras" },
            new OrderAccumulator.Domain.Entities.Symbol { Ticker = "VALE3", Description = "Vale" },
            new OrderAccumulator.Domain.Entities.Symbol { Ticker = "ITUB4", Description = "Itaú" }
        );
        db.SaveChanges();
    }
}

app.MapGet("/api/symbols", async (GetSymbolsUseCase useCase) =>
    Results.Ok(await useCase.ExecuteAsync()));

app.MapGet("/api/exposed", async (GetExposureUseCase useCase) =>
    Results.Ok(await useCase.ExecuteAsync()));

var fixAcceptor = app.Services.GetRequiredService<FixAcceptor>();
var settings = new SessionSettings("config/client.cfg");
var storeFactory = new FileStoreFactory(settings);
var logFactory = new ScreenLogFactory(settings);

var initiator = new ThreadedSocketAcceptor(
                        fixAcceptor, 
                        storeFactory, 
                        settings, 
                        logFactory);

initiator.Start();

app.Run();

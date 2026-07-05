using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderGenerator.Domain.Interfaces;
using OrderGenerator.Infrastructure.Configuration;
using OrderGenerator.Infrastructure.Persistence;

namespace OrderGenerator.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<ExchangeSettings>? configureExchangeSettings = null)
    {
        if (configureExchangeSettings is not null)
            services.Configure(configureExchangeSettings);

        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlite("Data Source=orders.db"));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}

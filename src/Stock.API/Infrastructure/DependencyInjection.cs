using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.API.Application.Interfaces;
using Stock.API.Domain.Interfaces;
using Stock.API.Infrastructure.Database;
using Stock.API.Infrastructure.Repositories;
using Stock.API.Infrastructure.Services;

namespace Stock.API.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IStockRepository, StockRepository>();

        return services;
    }
}

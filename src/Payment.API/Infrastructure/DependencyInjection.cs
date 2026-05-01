using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.API.Application.Interfaces;
using Payment.API.Infrastructure.Consumers;
using Payment.API.Infrastructure.Services;
using Shared;

namespace Payment.API.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<StockReservedEventConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"]);
                cfg.ReceiveEndpoint(RabbitMQSettings.Payment_StockReservedEventQueue, e =>
                {
                    e.ConfigureConsumer<StockReservedEventConsumer>(context);
                });
            });
        });

        return services;
    }
}

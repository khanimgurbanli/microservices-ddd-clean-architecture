using Microsoft.Extensions.DependencyInjection;

namespace Payment.API.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}

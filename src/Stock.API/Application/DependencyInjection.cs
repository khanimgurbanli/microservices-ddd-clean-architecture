using Microsoft.Extensions.DependencyInjection;

namespace Stock.API.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}

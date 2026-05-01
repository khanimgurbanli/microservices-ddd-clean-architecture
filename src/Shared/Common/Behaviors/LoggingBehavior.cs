using MediatR;
using Microsoft.Extensions.Logging;

namespace Shared.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("[START] Handling {RequestName}", requestName);

        try
        {
            var response = await next();

            _logger.LogInformation("[END] Handled {RequestName} successfully", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ERROR] Handling {RequestName} failed", requestName);
            throw;
        }
    }
}
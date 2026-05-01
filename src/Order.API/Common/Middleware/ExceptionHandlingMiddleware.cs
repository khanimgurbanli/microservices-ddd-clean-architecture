using FluentValidation;
using MediatR;

namespace Order.API.Presentation.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationException => (
                StatusCode: StatusCodes.Status400BadRequest,
                Message: string.Join("; ", validationException.Errors.Select(e => e.ErrorMessage))),
            InvalidOperationException => (
                StatusCode: StatusCodes.Status404NotFound,
                Message: exception.Message),
            _ => (
                StatusCode: StatusCodes.Status500InternalServerError,
                Message: "An internal server error occurred")
        };

        context.Response.StatusCode = response.StatusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            error = response.Message,
            statusCode = response.StatusCode
        });
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
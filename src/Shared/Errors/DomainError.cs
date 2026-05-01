namespace Shared.Errors;

public sealed record DomainError
{
    public static DomainError None => new(string.Empty, ErrorType.None);
    public static DomainError NotFound(string? message = null) => new(message ?? "The requested item could not be found.", ErrorType.NotFound);
    public static DomainError BadRequest(string? message = null) => new(message ?? "Invalid request or parameters.", ErrorType.BadRequest);
    public static DomainError Conflict(string? message = null) => new(message ?? "The data provided conflicts with existing data.", ErrorType.Conflict);
    public static DomainError Validation(string? message = null, IReadOnlyList<string>? errors = null) => new(message ?? "Validation failed.", ErrorType.Validation, errors);
    public static DomainError Unexpected(string? message = null) => new(message ?? "An unexpected error occurred.", ErrorType.Unexpected);

    public string Message { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyList<string>? Errors { get; }

    private DomainError(string message, ErrorType errorType, IReadOnlyList<string>? errors = null)
    {
        Message = message;
        ErrorType = errorType;
        Errors = errors;
    }
}

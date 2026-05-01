namespace Shared.Errors;

public abstract class ErrorType
{
    public static readonly ErrorType None = new NoneType();
    public static readonly ErrorType NotFound = new NotFoundType();
    public static readonly ErrorType BadRequest = new BadRequestType();
    public static readonly ErrorType Conflict = new ConflictType();
    public static readonly ErrorType Validation = new ValidationType();
    public static readonly ErrorType Unexpected = new UnexpectedType();

    public string Name { get; }
    public int Value { get; }

    protected ErrorType(string name, int value)
    {
        Name = name;
        Value = value;
    }

    private class NoneType : ErrorType { public NoneType() : base("None", 0) { } }
    private class NotFoundType : ErrorType { public NotFoundType() : base("NotFound", 1) { } }
    private class BadRequestType : ErrorType { public BadRequestType() : base("BadRequest", 2) { } }
    private class ConflictType : ErrorType { public ConflictType() : base("Conflict", 3) { } }
    private class ValidationType : ErrorType { public ValidationType() : base("Validation", 4) { } }
    private class UnexpectedType : ErrorType { public UnexpectedType() : base("Unexpected", 5) { } }
}

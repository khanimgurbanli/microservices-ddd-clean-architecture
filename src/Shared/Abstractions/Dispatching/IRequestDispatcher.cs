namespace Shared.Abstractions.Dispatching;

public interface IRequestDispatcher
{
    Task<TResponse> Send<TResponse>(object request, CancellationToken cancellationToken = default);
}

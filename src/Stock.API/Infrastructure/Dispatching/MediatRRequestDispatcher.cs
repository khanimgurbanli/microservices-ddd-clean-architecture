using MediatR;
using Shared.Abstractions.Dispatching;

namespace Stock.API.Infrastructure.Dispatching;

public class MediatRRequestDispatcher : IRequestDispatcher
{
    private readonly IMediator _mediator;

    public MediatRRequestDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> Send<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        return _mediator.Send((dynamic)request, cancellationToken);
    }
}

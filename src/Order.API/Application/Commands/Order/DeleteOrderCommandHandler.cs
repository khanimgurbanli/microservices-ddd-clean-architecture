using MediatR;
using Order.API.Application.Interfaces;
using Order.API.Domain.ValueObjects;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, CSharpFunctionalExtensions.Result<bool, DomainError>>
{
    private readonly IOrderService _orderService;

    public DeleteOrderCommandHandler(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<CSharpFunctionalExtensions.Result<bool, DomainError>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var result = await _orderService.DeleteAsync(OrderId.From(request.Id));
        if (result.IsFailure)
            return CSharpFunctionalExtensions.Result.Failure<bool, DomainError>(result.Error);

        return CSharpFunctionalExtensions.Result.Success<bool, DomainError>(true);
    }
}

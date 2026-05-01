using MediatR;
using Shared.Errors;

namespace Order.API.Application.Commands.Order;

public record DeleteOrderCommand(Guid Id) : IRequest<CSharpFunctionalExtensions.Result<bool, DomainError>>;

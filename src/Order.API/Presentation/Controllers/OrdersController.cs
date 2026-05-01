using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Order.API.Application.Commands.Order;
using Order.API.Application.Commands.Order.Models;
using Order.API.Application.Models.Orders;
using Order.API.Application.Queries.Order;
using Shared.Errors;
using Shared.Abstractions.Dispatching;

namespace Order.API.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IRequestDispatcher _dispatcher;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IRequestDispatcher dispatcher, ILogger<OrdersController> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders()
    {
        var result = await _dispatcher.Send<Result<IEnumerable<OrderResponse>, DomainError>>(new GetAllOrdersQuery());

        if (result.IsFailure)
            return MapErrorToObjectResult(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(Guid id)
    {
        var result = await _dispatcher.Send<Result<OrderResponse, DomainError>>(new GetOrderByIdQuery(id));

        if (result.IsFailure)
            return MapErrorToObjectResult(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderCommand createOrderCommand)
    {
        if (createOrderCommand == null)
            return BadRequest(new { error = "Order data cannot be null" });

        if (createOrderCommand.OrderItems == null || createOrderCommand.OrderItems.Count == 0)
            return BadRequest(new { error = "Order must contain at least one item" });

        var result = await _dispatcher.Send<Result<OrderResponse, DomainError>>(createOrderCommand);

        if (result.IsFailure)
            return MapErrorToObjectResult(result.Error);

        return CreatedAtAction(nameof(GetOrder), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<OrderResponse>> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
    {
        if (request == null)
            return BadRequest(new { error = "Order cannot be null" });

        var result = await _dispatcher.Send<Result<OrderResponse, DomainError>>(new UpdateOrderCommand(id, request.OrderStatus));

        if (result.IsFailure)
            return MapErrorToObjectResult(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var result = await _dispatcher.Send<Result<bool, DomainError>>(new DeleteOrderCommand(id));

        if (result.IsFailure)
            return MapErrorToObjectResult(result.Error);

        return Ok(new { message = $"Order with id {id} deleted successfully" });
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<OrderResponse>> ChangeOrderStatus(Guid id, [FromBody] int status)
    {
        var result = await _dispatcher.Send<Result<OrderResponse, DomainError>>(new ChangeOrderStatusCommand(id, status));

        if (result.IsFailure)
            return MapErrorToObjectResult(result.Error);

        return Ok(result.Value);
    }

    private ObjectResult MapErrorToObjectResult(DomainError error)
    {
        var statusCode = error.ErrorType switch
        {
            var e when e == ErrorType.NotFound => 404,
            var e when e == ErrorType.BadRequest => 400,
            var e when e == ErrorType.Validation => 400,
            var e when e == ErrorType.Conflict => 409,
            _ => 500
        };

        return StatusCode(statusCode, new { error = error.Message, errors = error.Errors });
    }
}

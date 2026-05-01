using Microsoft.EntityFrameworkCore;
using Order.API.Domain.Aggregates;
using Order.API.Domain.Interfaces;
using Order.API.Domain.ValueObjects;
using Order.API.Infrastructure.Persistence;
using OrderEntity = Order.API.Infrastructure.Database.Entities.OrderEntity;
using OrderItemEntity = Order.API.Infrastructure.Database.Entities.OrderItemEntity;

namespace Order.API.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<OrderAggregate?> GetByIdAsync(OrderId id)
    {
        var entity = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id.Value);

        return entity == null ? null : MapToAggregate(entity);
    }

    public async Task<IEnumerable<OrderAggregate>> GetAllAsync()
    {
        var entities = await _context.Orders
            .Include(o => o.OrderItems)
            .ToListAsync();

        return entities.Select(MapToAggregate);
    }

    public async Task AddAsync(OrderAggregate order)
    {
        var entity = new OrderEntity
        {
            Id = order.Id,
            BuyerId = order.BuyerId,
            TotalPrice = order.TotalPrice,
            OrderStatus = order.OrderStatus,
            CreatedDate = order.CreatedDate,
            OrderItems = order.Items.Select(i => new OrderItemEntity
            {
                Id = i.Id,
                OrderId = order.Id,
                ProductId = i.ProductId,
                Count = i.Count,
                Price = i.Price
            }).ToList()
        };

        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderAggregate order)
    {
        var existingEntity = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        if (existingEntity == null)
            throw new InvalidOperationException($"Order with id {order.Id} not found");

        existingEntity.BuyerId = order.BuyerId;
        existingEntity.TotalPrice = order.TotalPrice;
        existingEntity.OrderStatus = order.OrderStatus;

        // Update items
        existingEntity.OrderItems.Clear();
        foreach (var item in order.Items)
        {
            existingEntity.OrderItems.Add(new OrderItemEntity
            {
                Id = item.Id,
                OrderId = order.Id,
                ProductId = item.ProductId,
                Count = item.Count,
                Price = item.Price
            });
        }

        _context.Orders.Update(existingEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(OrderId id)
    {
        var entity = await _context.Orders.FindAsync(id.Value);
        if (entity != null)
        {
            _context.Orders.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    private static OrderAggregate MapToAggregate(OrderEntity entity)
    {
        var items = entity.OrderItems.Select(i => OrderItemAggregate.Load(
            i.Id,
            i.OrderId,
            i.ProductId,
            i.Count,
            i.Price
        )).ToList();

        var order = OrderAggregate.Load(
            entity.Id,
            entity.BuyerId,
            entity.OrderStatus,
            entity.TotalPrice,
            entity.CreatedDate,
            items
        );

        return order;
    }
}

using OrderGenerator.Domain.Entities;

namespace OrderGenerator.Domain.Interfaces;

/// <summary>
/// Repository interface for managing the persistence of stock orders.
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
}

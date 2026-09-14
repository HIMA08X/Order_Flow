using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order_Flow.Domain.Entities;

namespace Order_Flow.App.Orders.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken);
        Task<List<Order>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Order>> GetPendingAsync(CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}

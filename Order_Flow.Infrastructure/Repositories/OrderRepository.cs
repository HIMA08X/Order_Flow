using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Order_Flow.Domain.Entities;
using Order_Flow.Infrastructure.Data;
using Order_Flow.App.Orders.Interfaces;

namespace Order_Flow.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Order?> GetByIdAsync(int orderId, CancellationToken cancellationToken)
        {

            var order = await _context.Orders.FindAsync(new object[] { orderId }, cancellationToken);
            return order;
        }

        public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Orders.Include(x => x.Items).AsNoTracking().ToListAsync(cancellationToken);
        }

        public Task<List<Order>> GetPendingAsync(CancellationToken cancellationToken)
        {
          return _context.Orders.Where(x => x.Status == "Pending").ToListAsync(cancellationToken);
        }
    }
}

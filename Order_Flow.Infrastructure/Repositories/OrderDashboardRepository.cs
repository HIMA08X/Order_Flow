using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order_Flow.App.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Order_Flow.Infrastructure.Data;
using Order_Flow.Infrastructure.ReadModels;


namespace Order_Flow.Infrastructure.Repositories
{
    public class OrderDashboardRepository : IOrderDashboardRepository
    {
        private readonly AppDbContext _dbContext;
        public OrderDashboardRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<OrderDashboardItem_DTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.OrderDashboardReadModels.AsNoTracking().OrderByDescending(x => x.OrderId).Select(x => new OrderDashboardItem_DTO
            { 
                OrderId = x.OrderId,
                CustomerName = x.CustomerName,
                ItemCount = x.ItemCount,
                Total = x.Total,
                Status = x.Status 
             }).ToListAsync(cancellationToken);
        }
        public async Task RefreshAsync(CancellationToken cancellationToken)
        {
            var orders = await _dbContext.Orders.Include(x => x.Customer).Include(x => x.Items).AsNoTracking().ToListAsync(cancellationToken);
            await _dbContext.OrderDashboardReadModels.ExecuteDeleteAsync(cancellationToken);
            var readModels = orders.Select(o => new OrderDashboardReadModel
            {
                OrderId = o.Id,
                CustomerName = o.Customer.Name,
                ItemCount = o.Items.Count,
                Total = o.Total,
                Status = o.Status
            }).ToList();
            await _dbContext.OrderDashboardReadModels.AddRangeAsync(readModels, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

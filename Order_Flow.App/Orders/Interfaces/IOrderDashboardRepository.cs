using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Interfaces
{
    public interface  IOrderDashboardRepository 
    {
        Task<List<OrderDashboardItem_DTO>> GetAllAsync(CancellationToken cancellationToken);
        Task RefreshAsync(CancellationToken cancellationToken);

    }
}

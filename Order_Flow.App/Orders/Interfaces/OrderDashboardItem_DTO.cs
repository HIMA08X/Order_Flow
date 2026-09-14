using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Interfaces
{
    public record OrderDashboardItem_DTO
    {
        public int OrderId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public int ItemCount { get; init; }
        public decimal Total { get; init; }
        public string Status { get; init; } = string.Empty;
        
    }
}

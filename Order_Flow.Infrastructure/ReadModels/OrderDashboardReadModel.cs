using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.Infrastructure.ReadModels
{
    public class OrderDashboardReadModel
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdateAt { get; set; }
    }
}

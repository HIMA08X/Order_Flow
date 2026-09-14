using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Order_Flow.App.Orders.Interfaces;

namespace Order_Flow.App.Orders.Queries.Get_Dashboard_Orders
{
    public record Get_Dashboard_Orders_Query : IRequest<List<OrderDashboardItem_DTO>>;
    
}

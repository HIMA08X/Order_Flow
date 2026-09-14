using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Order_Flow.App.Orders.Interfaces;

namespace Order_Flow.App.Orders.Queries.Get_Dashboard_Orders
{
    internal class Get_Dashboard_Orders_Query_Handler : IRequestHandler<Get_Dashboard_Orders_Query, List<OrderDashboardItem_DTO>>
    {
        private readonly IOrderDashboardRepository _orderRepository;
        public Get_Dashboard_Orders_Query_Handler(IOrderDashboardRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        
        public async Task<List<OrderDashboardItem_DTO>> Handle(Get_Dashboard_Orders_Query request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetAllAsync(cancellationToken);
        }
    }
}

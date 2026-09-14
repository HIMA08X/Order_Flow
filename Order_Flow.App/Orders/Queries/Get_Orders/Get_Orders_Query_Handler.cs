using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order_Flow.App.Orders.Interfaces;
using Order_Flow.App.Orders.Queries.Get_Orders;
using MediatR;


namespace Order_Flow.App.Orders.Queries.Get_Orders
{
    public class Get_Orders_Query_Handler : IRequestHandler<Get_Orders_Query, List<Order_List_Item_DTO>>
    {
        private readonly IOrderRepository _orderRepository;
        public Get_Orders_Query_Handler(IOrderRepository orderRepository )
        {
            _orderRepository = orderRepository;
        }
        public async Task<List<Order_List_Item_DTO>> Handle(Get_Orders_Query request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAllAsync(cancellationToken);
            var orderList = orders.Select(o => new Order_List_Item_DTO(
                o.Id,
                o.CustomerId,
                o.Items.Count,
                o.Total,
                o.Status
            )).ToList();
            return orderList;
        }

    }
}

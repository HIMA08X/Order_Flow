using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Order_Flow.App.Orders.Interfaces;
using Order_Flow.App.Orders.Queries;


namespace Order_Flow.App.Orders.Queries
{
    internal class Get_Order_By_Id_Query_Handler : IRequestHandler<Get_Order_By_Id_Query, Order_Details_DTO>
    {
        private readonly  IOrderRepository _orderRepository;

        public Get_Order_By_Id_Query_Handler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        private readonly IOrderCacheService _cacheService;
        public Get_Order_By_Id_Query_Handler(IOrderRepository orderRepository, IOrderCacheService cacheService)
        {
            _orderRepository = orderRepository;
            _cacheService = cacheService;
        }

        public async Task<Order_Details_DTO> Handle(Get_Order_By_Id_Query request, CancellationToken cancellationToken)
        {
            var cacheKey = $"order:{request.OrderId}";
            var cachedOrder = await _cacheService.GetAsync<Order_Details_DTO>(cacheKey, cancellationToken);
            if (cachedOrder != null)
            {
                return cachedOrder;
            }
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                return null;
            }
               
            
            var result = new Order_Details_DTO(
                order.Id,
                order.CustomerId,
                order.Status,
                order.Total,
                order.Items.Select(item => new Order_Item_Details_DTO(
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.Quantity * item.UnitPrice)).ToList()
            );
            await _cacheService.SetAsync(cacheKey,result, TimeSpan.FromMinutes(10), cancellationToken);
            return result;
        }

    }
}

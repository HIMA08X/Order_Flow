using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Order_Flow.Domain.Entities;
using System.Threading.Tasks;
using MediatR;
using Order_Flow.App.Orders.Interfaces;


namespace Order_Flow.App.Orders.Commands.Create_Order
{
    internal class Create_Order_Command_Handler : IRequestHandler<Create_Order_Command, int>
    {
        private readonly IOrderRepository _orderRepository;
        
        public Create_Order_Command_Handler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        public async Task<int> Handle(Create_Order_Command _request, CancellationToken _cancellationToken)
        {
            if(_request.Items == null || _request.Items.Count == 0) 
                throw new ArgumentException("Order must contain at least one item.");

            var order = new Order(_request.CustomerId);
            foreach (var item in _request.Items)
            {
               var orderItem = new Order_Item
                    (
                      item.ProductName,
                        item.Quantity,
                        item.UnitPrice
                    );
                order.AddItem(orderItem);
            }
            await _orderRepository.AddAsync(order, _cancellationToken);
            await _orderRepository.SaveChangesAsync(_cancellationToken);
            return order.Id;
            
        }
    }
}

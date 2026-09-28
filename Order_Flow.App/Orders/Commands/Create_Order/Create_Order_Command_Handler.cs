using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Order_Flow.Domain.Entities;
using System.Threading.Tasks;
using MediatR;
using Order_Flow.App.Orders.Interfaces;
using System.Diagnostics;
using Order_Flow.App.Observability;
using Microsoft.Extensions.Logging;


namespace Order_Flow.App.Orders.Commands.Create_Order
{
    internal class Create_Order_Command_Handler : IRequestHandler<Create_Order_Command, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<Create_Order_Command_Handler> _logger;
        
        public Create_Order_Command_Handler(IOrderRepository orderRepository , ILogger<Create_Order_Command_Handler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }


        public async Task<int> Handle(Create_Order_Command _request, CancellationToken _cancellationToken)
        {
            using var activity = OrderFlowTracing.ActivitySource.StartActivity("OrderFlow.CreateOrder");
            activity?.SetTag("order.customer_id", _request.CustomerId);
            activity?.SetTag("order.items_count", _request.Items?.Count ?? 0);

            if (_request.Items == null || _request.Items.Count == 0)
            {
                _logger.LogWarning("Order creation failed because the order contains no items. CustomerId:{CustomerId}", _request.CustomerId);
                throw new ArgumentException("Order must contain at least one item.");
            }
            try
            {
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
                OrderFlowMetrics.OrdersCreated.Add(1);

                OrderFlowMetrics.IncrementPendingOrders();

                activity?.SetTag(
                    "order.id",
                    order.Id);

                _logger.LogInformation(
                    "Order created successfully. OrderId: {OrderId}, CustomerId: {CustomerId}",
                    order.Id,
                    _request.CustomerId);
                return order.Id;
            }
            catch (Exception exception)
            {
                activity?.SetStatus(
                    ActivityStatusCode.Error,
                    exception.Message);

                _logger.LogError(
                    exception,
                    "Error occurred while creating order. CustomerId: {CustomerId}",
                    _request.CustomerId);

                throw;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order_Flow.App.Orders.Interfaces;
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Order_Flow.App.Observability;
using Order_Flow.App.Orders.Interfaces;

namespace Order_Flow.App.Orders.Queries.Get_Orders;

public class Get_Orders_Query_Handler
    : IRequestHandler<Get_Orders_Query, List<Order_List_Item_DTO>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<Get_Orders_Query_Handler> _logger;

    public Get_Orders_Query_Handler(
        IOrderRepository orderRepository,
        ILogger<Get_Orders_Query_Handler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<List<Order_List_Item_DTO>> Handle(
        Get_Orders_Query request,
        CancellationToken cancellationToken)
    {
        using var activity =
            OrderFlowTracing.ActivitySource.StartActivity(
                "OrderFlow.GetOrders");

        try
        {
            var orders =
                await _orderRepository.GetAllAsync(
                    cancellationToken);

            var orderList =
                orders.Select(
                    order =>
                        new Order_List_Item_DTO(
                            order.Id,
                            order.CustomerId,
                            order.Items.Count,
                            order.Total,
                            order.Status))
                .ToList();

            activity?.SetTag(
                "orders.count",
                orderList.Count);

            _logger.LogInformation(
                "Orders retrieved successfully. Count: {Count}",
                orderList.Count);

            return orderList;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(
                ActivityStatusCode.Error,
                exception.Message);

            _logger.LogError(
                exception,
                "Error occurred while retrieving orders.");

            throw;
        }
    }
}
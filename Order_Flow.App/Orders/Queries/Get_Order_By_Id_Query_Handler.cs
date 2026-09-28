using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Order_Flow.App.Observability;
using Order_Flow.App.Orders.Interfaces;
namespace Order_Flow.App.Orders.Queries;

internal class Get_Order_By_Id_Query_Handler
    : IRequestHandler<Get_Order_By_Id_Query, Order_Details_DTO>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderCacheService _cacheService;
    private readonly ILogger<Get_Order_By_Id_Query_Handler> _logger;

    public Get_Order_By_Id_Query_Handler(
        IOrderRepository orderRepository,
        IOrderCacheService cacheService,
        ILogger<Get_Order_By_Id_Query_Handler> logger)
    {
        _orderRepository = orderRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Order_Details_DTO> Handle(
        Get_Order_By_Id_Query request,
        CancellationToken cancellationToken)
    {
        using var activity =
            OrderFlowTracing.ActivitySource.StartActivity(
                "OrderFlow.GetOrderById");

        activity?.SetTag(
            "order.id",
            request.OrderId);

        try
        {
            var cacheKey =
                $"order:{request.OrderId}";

            var cachedOrder =
                await _cacheService.GetAsync<Order_Details_DTO>(
                    cacheKey,
                    cancellationToken);

            if (cachedOrder != null)
            {
                activity?.SetTag(
                    "order.cache",
                    "hit");

                _logger.LogInformation(
                    "Cache hit while retrieving order. OrderId: {OrderId}",
                    request.OrderId);

                return cachedOrder;
            }

            activity?.SetTag(
                "order.cache",
                "miss");

            _logger.LogInformation(
                "Cache miss while retrieving order. OrderId: {OrderId}",
                request.OrderId);

            var order =
                await _orderRepository.GetByIdAsync(
                    request.OrderId,
                    cancellationToken);

            if (order == null)
            {
                _logger.LogWarning(
                    "Order was not found. OrderId: {OrderId}",
                    request.OrderId);

                activity?.SetTag(
                    "order.result",
                    "not_found");

                return null;
            }

            var result =
                new Order_Details_DTO(
                    order.Id,
                    order.CustomerId,
                    order.Status,
                    order.Total,
                    order.Items.Select(
                        item =>
                            new Order_Item_Details_DTO(
                                item.ProductName,
                                item.Quantity,
                                item.UnitPrice,
                                item.Quantity * item.UnitPrice))
                        .ToList());

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            activity?.SetTag(
                "order.result",
                "database");

            _logger.LogInformation(
                "Order retrieved successfully. OrderId: {OrderId}",
                request.OrderId);

            return result;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(
                ActivityStatusCode.Error,
                exception.Message);

            _logger.LogError(
                exception,
                "Error occurred while retrieving order. OrderId: {OrderId}",
                request.OrderId);

            throw;
        }
    }
}
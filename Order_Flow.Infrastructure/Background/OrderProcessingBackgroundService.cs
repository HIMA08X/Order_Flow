using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Order_Flow.App.Observability;
using Order_Flow.App.Orders.Interfaces;

namespace Order_Flow.Infrastructure.Background;

public class OrderProcessingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderProcessingBackgroundService> _logger;

    public OrderProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<OrderProcessingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            OrderFlowMetrics.WorkerCycles.Add(1);

            try
            {
                using var scope =
                    _serviceProvider.CreateScope();

                var orderRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IOrderRepository>();

                var dashboardRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IOrderDashboardRepository>();

                var cacheService =
                    scope.ServiceProvider
                        .GetRequiredService<IOrderCacheService>();

                var pendingOrders =
                    await orderRepository.GetPendingAsync(
                        stoppingToken);

                OrderFlowMetrics.SetPendingOrders(
                    pendingOrders.Count);

                _logger.LogInformation(
                    "Background worker cycle started. PendingOrders: {PendingOrders}",
                    pendingOrders.Count);

                foreach (var order in pendingOrders)
                {
                    order.Complet();

                    await cacheService.RemoveAsync(
                        $"order:{order.Id}",
                        stoppingToken);

                    _logger.LogInformation(
                        "Background worker processed order. OrderId: {OrderId}",
                        order.Id);
                }

                if (pendingOrders.Count > 0)
                {
                    await orderRepository.SaveChangesAsync(
                        stoppingToken);

                    OrderFlowMetrics.WorkerOrdersProcessed.Add(
                        pendingOrders.Count);
                }

                await dashboardRepository.RefreshAsync(
                    stoppingToken);

                var remainingPendingOrders =
                    await orderRepository.GetPendingAsync(
                        stoppingToken);

                OrderFlowMetrics.SetPendingOrders(
                    remainingPendingOrders.Count);

                _logger.LogInformation(
                    "Background worker cycle completed. ProcessedOrders: {ProcessedOrders}, RemainingPendingOrders: {RemainingPendingOrders}",
                    pendingOrders.Count,
                    remainingPendingOrders.Count);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error occurred during background worker processing.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(30),
                stoppingToken);
        }
    }
}
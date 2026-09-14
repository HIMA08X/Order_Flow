using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order_Flow.App.Orders.Interfaces;

namespace Order_Flow.Infrastructure.Background
{
    public class OrderProcessingBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _scopeFactory;
        public OrderProcessingBackgroundService(IServiceProvider scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                    var dashboardRepository = scope.ServiceProvider.GetRequiredService<IOrderDashboardRepository>();
                    var cacheService = scope.ServiceProvider.GetRequiredService<IOrderCacheService>();
                    var pendingOrders = await orderRepository.GetPendingAsync(stoppingToken);
                    foreach (var order in pendingOrders)
                    {
                        order.Complet();
                        await cacheService.RemoveAsync($"order_{order.Id}", stoppingToken);
                    }
                    if(pendingOrders.Count > 0)
                    {
                        await orderRepository.SaveChangesAsync(stoppingToken);
                    }


                    await dashboardRepository.RefreshAsync(stoppingToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Adjust the delay as needed
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Order_Flow.App.Orders.Interfaces;
using Order_Flow.Infrastructure.Background;
using Order_Flow.Infrastructure.Caching;
using Order_Flow.Infrastructure.Data;
using Order_Flow.Infrastructure.Repositories;
namespace Order_Flow.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var redisConnection = builder.Configuration.GetConnectionString("Redis");
            builder.Services.AddControllers();


            builder.Services.AddAuthorization();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "OrderFlow:";
            });

            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(
                    typeof(Order_Flow.App.Orders.Commands.Create_Order.Create_Order_Command)
                        .Assembly);
            });

            builder.Services.AddScoped<
                    IOrderRepository,
                    OrderRepository>();
                  
            builder.Services.AddScoped<
                    IOrderDashboardRepository,
                    OrderDashboardRepository>();

            builder.Services.AddScoped<
                    IOrderCacheService,
                    RedisOrderCacheService>();

            builder.Services.AddHostedService<
                    OrderProcessingBackgroundService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

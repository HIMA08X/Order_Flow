using Microsoft.EntityFrameworkCore;
using Order_Flow.App.Orders.Interfaces;
using Order_Flow.Infrastructure.Background;
using Order_Flow.Infrastructure.Caching;
using Order_Flow.Infrastructure.Data;
using Order_Flow.Infrastructure.Repositories;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using OpenTelemetry.Resources;
using Order_Flow.API.Middleware;
using Order_Flow.App.Observability;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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

            builder.Logging.Configure(options =>
            {
                options.ActivityTrackingOptions =
                    ActivityTrackingOptions.TraceId |
                    ActivityTrackingOptions.SpanId |
                    ActivityTrackingOptions.ParentId;
            });

            builder.Services
                .AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService("OrderFlow");
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddMeter(OrderFlowMetrics.MeterName)
                        .AddConsoleExporter(
                            (exporterOptions, metricReaderOptions) =>
                            {
                                metricReaderOptions
                                    .PeriodicExportingMetricReaderOptions
                                    .ExportIntervalMilliseconds = 5000;
                            })
                        .AddPrometheusExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddSource(OrderFlowTracing.ActivitySourceName)
                        .AddAspNetCoreInstrumentation()
                        .AddConsoleExporter();
                });



            builder.Services.AddAuthorization();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            builder.Services.AddHealthChecks().AddCheck("application" , () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()).AddSqlServer(connectionString!, name: "sql-server").AddRedis(redisConnection!, name: "redis");
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
            app.UseMiddleware<RequestMetricsMiddleware>();
            app.UseAuthorization();
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            duration = entry.Value.Duration.ToString()
                        })
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(response));
                }
            });
            app.MapPrometheusScrapingEndpoint("/metrics");
            app.MapControllers();

            app.Run();
        }
    }
}

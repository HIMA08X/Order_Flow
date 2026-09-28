using System.Diagnostics;
using Order_Flow.App.Observability;


namespace Order_Flow.API.Middleware
{
    public class RequestMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestMetricsMiddleware> _logger;
        public RequestMetricsMiddleware(RequestDelegate next , ILogger<RequestMetricsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var tags = new TagList
        {
            { "http.method", context.Request.Method }
        };

            var exceptionThrown = false;

            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                exceptionThrown = true;

                OrderFlowMetrics.HttpErrors.Add(1, tags);

                _logger.LogError(
                    exception,
                    "Unhandled HTTP error. Method: {Method}, Path: {Path}",
                    context.Request.Method,
                    context.Request.Path);

                throw;
            }
            finally
            {
                stopwatch.Stop();

                tags.Add(
                    "http.status_code",
                    context.Response.StatusCode);

                OrderFlowMetrics.HttpRequests.Add(1, tags);

                OrderFlowMetrics.RequestDuration.Record(
                    stopwatch.Elapsed.TotalMilliseconds,
                    tags);

                if (!exceptionThrown &&
                    context.Response.StatusCode >= 400)
                {
                    OrderFlowMetrics.HttpErrors.Add(1, tags);

                    if (context.Response.StatusCode >= 500)
                    {
                        _logger.LogError(
                            "HTTP server error. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}",
                            context.Request.Method,
                            context.Request.Path,
                            context.Response.StatusCode);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "HTTP client error. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}",
                            context.Request.Method,
                            context.Request.Path,
                            context.Response.StatusCode);
                    }
                }
            }
        }

    }
}

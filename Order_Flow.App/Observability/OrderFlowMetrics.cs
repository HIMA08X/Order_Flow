using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using System.Resources;

namespace Order_Flow.App.Observability
{
    public static class OrderFlowMetrics
    {
        public const string MeterName = "OrderFlow";
        public static readonly Meter Meter = new(MeterName);
        public static readonly Counter<long> HttpRequests = Meter.CreateCounter<long>("orderflow.http.requests", description: "Number of HTTP Requests");
        public static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>("orderflow.http.request.duration", unit: "ms", description: "HTTTP Request duration in ms");
        public static readonly Counter<long> HttpErrors = Meter.CreateCounter<long>("orderflow.http.errors", description: "Number of HTTP Errors");
        public static readonly Counter<long> OrdersCreated = Meter.CreateCounter<long>("orderflow.orders.created", description: "Number of orders created");
        private static long _pendingOrders;
        public static readonly ObservableGauge<long> PendingOrders = Meter.CreateObservableGauge<long>("orderflow.order.pending", () => Volatile.Read(ref _pendingOrders), description: "Current number of pending orders");
        public static readonly Counter<long> WorkerCycles = Meter.CreateCounter<long>("orderflow.worker.cycles", description: "Number of background worker cycles.");

        public static readonly Counter<long> WorkerOrdersProcessed = Meter.CreateCounter<long>("orderflow.worker.orders.processed", description: "Number of orders processed by the background worker.");

        public static void IncrementPendingOrders()
        {
            Interlocked.Increment(ref _pendingOrders);
        }

        public static void SetPendingOrders(long count)
        {
            Interlocked.Exchange(ref _pendingOrders, count);
        }

    }
}

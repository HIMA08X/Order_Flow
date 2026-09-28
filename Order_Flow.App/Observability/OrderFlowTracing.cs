using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace Order_Flow.App.Observability
{
    public static class OrderFlowTracing
    {
        public const string ActivitySourceName = "OrderFlow";
        public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    }
}

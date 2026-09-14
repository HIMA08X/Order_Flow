using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order_Flow.App.Orders.Interfaces
{
    public interface IOrderCacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
        Task SetAsync<T>(string key,T value, TimeSpan expiration, CancellationToken cancellationToken);
        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}

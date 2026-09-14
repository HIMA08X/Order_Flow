using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Order_Flow.App.Orders.Interfaces;


namespace Order_Flow.Infrastructure.Caching
{
    public class RedisOrderCacheService : IOrderCacheService
    {
        private readonly IDistributedCache _cache;
        public RedisOrderCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            if (cachedData == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(cachedData);
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken)
        {
            var data = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, data, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            }, cancellationToken);
        }
        public async Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            await _cache.RemoveAsync(key, cancellationToken);

        }
    }

}

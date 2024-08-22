

### Caching objects using in-memory caching

When you add an object to a cache, you should set an expiration. There are two types, absolute and sliding, and you can set one or the other, both, or neither:
2 types o chaching:
- absolute : This is a fixed date/time, for example, 1am on December 24, 2023. When the date/time is reached, the object is evicted. To use this, set the AbsoluteExpiration property of a cache entry to a DateTime value. Choose this if you need to guarantee that at some point the data in the cache will be refreshed.
- 
- sliding:  This is a time span, for example, 20 seconds. When the time span expires, the object is evicted. However, whenever an object is read from the cache, its expiration is reset for another 20 seconds. This is why it is described as sliding. 
- Never: You can set a cache entry to have a priority of CacheItemPriority.NeverRemove.

## Implementing the memeory cache

1. add `Microsoft.Extensions.Caching.Memory`. Has a modern implementation of `IMemoryCache`

```csharp

builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
  new MemoryCacheOptions
  {
    TrackStatistics = true,
    SizeLimit = 50 // Products.
  }));
```

-  To use `IMemoryCache`. in `ProductsController` and set it as a state on the class
- import using `Microsoft.Extensions.Caching.Memory;`
- add a method  for `LimitedStockProducts` 

```csharp

  // GET: api/products/limitedstock
  [HttpGet]
  [Route("limitedstock")]
  [Produces(typeof(Product[]))]
  public async Task<IEnumerable<Product>> GetLimitedStockProducts()
  {
      // Try to get the cached value.
      if (!memoryCache.TryGetValue(LimitedStockProductsKey, out Product[]? cachedValue))
      {


          // If the cached value is not found, get the value from the database.
          var products = await _productService.GetProductsAsync();
          cachedValue = products.Where(p => p.Stock <= 30)
            .ToArray();


          MemoryCacheEntryOptions cacheEntryOptions = new()
          { //AbsoluteExpiration = DateTimeOffset.UtcNow,
              SlidingExpiration = TimeSpan.FromSeconds(5),
              Size = cachedValue?.Length
          };

          memoryCache.Set(LimitedStockProductsKey, cachedValue, cacheEntryOptions);
      }
      MemoryCacheStatistics? stats = memoryCache.GetCurrentStatistics();
      logger.LogInformation($"Memory cache. Total hits: {stats?
            .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");
      return cachedValue ?? Enumerable.Empty<Product>();
  }
```

- add the keys

```csharp

private readonly ILogger<ProductsController> logger;
private const string LimitedStockProductsKey = "LSPC";
```

- Test by making some requests to the endpoint
- increase the sliding expiration to 120 seconds and do the request twice
- notice how the second time doesn't get the items from the database, but from the cache


## Caching objects using distributed caching
Distributed caches have benefits over in-memory caches. Cached objects:

- Are consistent across requests to multiple servers.
- Survive server restarts and service deployments.
- Do not waste local server memory and exhaust resources
- Are stored in a shared area, so in a server farm scenario with multiple servers, you do not need to enable sticky sessions.

Microsoft provides the `IDistributedCache` interface with pre-defined methods to manipulate items in any distributed cache implementation.
The methods are:
- Set or SetAsync: To store an object in the cache.
- Get or GetAsync: To retrieve an object from the cache.
- Remove or RemoveAsync: To remove an object from the cache.
- Refresh or RefreshAsync: To reset the sliding expiration for an object in the cache.

Implement it:
- add `using Microsoft.Extensions.Caching.Distributed;`
- add `builder.Services.AddDistributedMemoryCache();`

- add the fields at the controller level

```csharp
  private readonly IDistributedCache distributedCache;
  private const string OverstockedProductsKey = "OSPK";
```


```c#

private readonly IMemoryCache _memoryCache;
private const string OutOfStockProductsKey = "OOSP";
private readonly IDistributedCache _distributedCache;
private const string DiscontinuedProductsKey = "DISCP";
public ProductsController(ILogger<ProductsController> logger,
  NorthwindContext context,
  IMemoryCache memoryCache,
  IDistributedCache distributedCache)
{
  _logger = logger;
  _db = context;
  _memoryCache = memoryCache;
  _distributedCache = distributedCache;
}



//private method
   private Product[]? GetDiscontinuedProductsFromDatabase()
   {
       // If the cached value is not found, get the value from the database.
       var products = _productService.GetProductsAsync().Result;
       var cachedValue = products.Where(p => p.Stock > 100)
           .ToArray();


       DistributedCacheEntryOptions cacheEntryOptions = new()
       {
           // Allow readers to reset the cache entry's lifetime.
           SlidingExpiration = TimeSpan.FromSeconds(5),

           // Set an absolute expiration time for the cache entry.
           AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20),
       };

       byte[]? cachedValueBytes =
         JsonSerializer.SerializeToUtf8Bytes(cachedValue);

       distributedCache.Set(OverstockedProductsKey, cachedValueBytes, cacheEntryOptions);

       return cachedValue;
   }

    // GET: api/products/overstocked
    [HttpGet]
    [Route("overstocked")]
    [Produces(typeof(Product[]))]
    public async Task<IEnumerable<Product>> GetOverStockedProducts()
    {
        // Try to get the cached value.
        byte[]? cachedValueBytes = distributedCache.Get(OverstockedProductsKey);
        Product[]? cachedValue = null;
        if (cachedValueBytes is null)
        {
            cachedValue = GetDiscontinuedProductsFromDatabase();
        }
        else
        {
            cachedValue = JsonSerializer
              .Deserialize<Product[]?>(cachedValueBytes);
            if (cachedValue is null)
            {
                cachedValue = GetDiscontinuedProductsFromDatabase();
            }
        }
        return cachedValue ?? Enumerable.Empty<Product>();
    }
```

Unlike the in-memory cache that can store any live object, objects stored in distributed cache implementations must be serialized into byte arrays because they need to be transmittable across networks.


## Add response caching

```csharp
builder.Services.AddResponseCaching();

app.UseResponseCaching();


```

# Summary
Caching is one of the best ways to improve the performance and scalability of your services.


## Resources
- https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0#distributed-redis-cache
- https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0#distributed-sql-server-cache
- 
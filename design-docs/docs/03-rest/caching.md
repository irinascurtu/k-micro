# Add Caching
## What is caching?



## General guidelines
Caching is most effective for data that is expensive to generate and rarely changes. When implementing caching, follow these guidelines:

- Code Independence: Ensure your code can retrieve data from the original source if it's not in the cache.
- Resource Management: Cache storage is limited, so set expirations and size limits on cached data.
- Performance Monitoring: Track cache hits to fine-tune and balance caching strategies for optimal performance.

## Adding Caching to the API

### Caching web responses using HTTP caching

The Cache-Control HTTP header for requests and responses has some common directives.
| **Directive** | **Description**                                                                                      |
|---------------|------------------------------------------------------------------------------------------------------|
| **public**    | Clients and intermediaries can cache this response.                                                  |
| **private**   | Only a client should cache this response.                                                            |
| **max-age**   | The client does not accept responses older than the specified number of seconds.                    |
| **no-cache**  | A client request is asking for a non-cached response. A server is instructing not to cache the response. |
| **no-store**  | A cache must not store the request or response.                                                       |

Other headers that can affect caching:
| **Header** | **Description**                                                                                        |
|------------|--------------------------------------------------------------------------------------------------------|
| **Age**    | Estimated number of seconds old the response is.                                                      |
| **Expires**| An absolute date/time after which the response should be considered expired.                         |
| **Vary**   | All specified fields must match for a cached response to be sent; otherwise, a fresh response is provided. For example, a query string of color. |

To use response chaching decorate method or controller wit the `[ResponseCache]` attribute, and add `builder.Services.AddResponseCaching();`
This supports several properties:
| **Property**     | **Description**                                                                                       |
|------------------|-------------------------------------------------------------------------------------------------------|
| **Duration**     | How long to cache the response, specified in seconds.                                                 |
| **Location**     | Indicates where the response can be cached: Any (public), Client (private), or None (no-cache).       |
| **NoStore**      | Sets `cache-control: no-store` to prevent caching.                                                     |
| **VaryByHeader** | Sets the `Vary` header to specify which headers should be considered for caching.                      |
| **VaryByQueryKeys** | Specifies which query keys should be used to vary the cached response.                              |

Example:

```c#

// GET api/products/5
[HttpGet("{id:int}")]
[ResponseCache(Duration = 5, // Cache-Control: max-age=5
  Location = ResponseCacheLocation.Any, // Cache-Control: public
  VaryByHeader = "User-Agent" // Vary: User-Agent
  )]
public async ValueTask<Product?> Get(int id)
{
  return await _db.Products.FindAsync(id);
}
```

note:: If using CORS middleware, then UseCors must be called before `UseResponseCaching`.



The web server sets response caching headers, and then intermediate proxies and clients should respect the headers to tell them how they should cache the responses.

Requirements for HTTP aka response caching include the following:

-The request must be a GET or HEAD one. POST, PUT, and DELETE requests, and so on, are never cached by HTTP caching.
-The response must have a 200 OK status code.
-If the request has an Authorization header, then the response is not cached.
-If the request has a Vary header, then the response is not cached when the values are not valid or *.



### Caching objects using in-memory caching

When you add an object to a cache, you should set an expiration. There are two types, absolute and sliding, and you can set one or the other, both, or neither:
2 types o chaching:
- absolute : This is a fixed date/time, for example, 1am on December 24, 2023. When the date/time is reached, the object is evicted. To use this, set the AbsoluteExpiration property of a cache entry to a DateTime value. Choose this if you need to guarantee that at some point the data in the cache will be refreshed.
- sliding:  This is a time span, for example, 20 seconds. When the time span expires, the object is evicted. However, whenever an object is read from the cache, its expiration is reset for another 20 seconds. This is why it is described as sliding. 
- Never: You can set a cache entry to have a priority of CacheItemPriority.NeverRemove.

Implementing cache;
1. add `Microsoft.Extensions.Caching.Memory`. Has a modern implementation of `IMemoryCache`

```c#

builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
  new MemoryCacheOptions
  {
    TrackStatistics = true,
    SizeLimit = 50 // Products.
  }));
```

- import using `Microsoft.Extensions.Caching.Memory; /`/ To use IMemoryCache. in ProductsController and set it as a state on the clsas
- add cache for GetProducts by adjusting the example below

```c#

// GET: api/products/outofstock
[HttpGet]
[Route("outofstock")]
[Produces(typeof(Product[]))]
public IEnumerable<Product> GetOutOfStockProducts()
{
  // Try to get the cached value.
  if (!_memoryCache.TryGetValue(OutOfStockProductsKey,
    out Product[]? cachedValue))
  {
    // If the cached value is not found, get the value from the database.
    cachedValue = _db.Products
      .Where(p => p.UnitsInStock == 0 && !p.Discontinued)
      .ToArray();
    MemoryCacheEntryOptions cacheEntryOptions = new()
    {
      SlidingExpiration = TimeSpan.FromSeconds(5),
      Size = cachedValue?.Length
    };
    _memoryCache.Set(OutOfStockProductsKey, cachedValue, cacheEntryOptions);
  }
  MemoryCacheStatistics? stats = _memoryCache.GetCurrentStatistics();
  _logger.LogInformation("Memory cache. Total hits: {stats?
    .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");
  return cachedValue ?? Enumerable.Empty<Product>();
}
```

- Test by making some requests to the endpoint

### Caching objects using distributed caching
Distributed caches have benefits over in-memory caches. Cached objects:

- Are consistent across requests to multiple servers.
- Survive server restarts and service deployments.
- Do not waste local server memory and exhaust resources
- Are stored in a shared area, so in a server farm scenario with multiple servers, you do not need to enable sticky sessions.

Microsoft provides the `IDistributedCache` interface with pre-defined methods to manipulate items in any distributed cache implementation.
The methods are:
-Set or SetAsync: To store an object in the cache.
-Get or GetAsync: To retrieve an object from the cache.
-Remove or RemoveAsync: To remove an object from the cache.
-Refresh or RefreshAsync: To reset the sliding expiration for an object in the cache.

Implement it:
- add `using Microsoft.Extensions.Caching.Distributed;`
- add `builder.Services.AddDistributedMemoryCache();`
- 
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
  Product[]? cachedValue = _db.Products
    .Where(product => product.Discontinued)
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
  _distributedCache.Set(DiscontinuedProductsKey,
    cachedValueBytes, cacheEntryOptions);
  return cachedValue;
}


// GET: api/products/discontinued
[HttpGet]
[Route("discontinued")]
[Produces(typeof(Product[]))]
public IEnumerable<Product> GetDiscontinuedProducts()
{
  // Try to get the cached value.
  byte[]? cachedValueBytes = _distributedCache.Get(DiscontinuedProductsKey);
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

# Summary
Caching is one of the best ways to improve the performance and scalability of your services.


## Resources
- https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0#distributed-redis-cache
- https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-8.0#distributed-sql-server-cache
- 
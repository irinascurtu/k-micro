using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using ProductsApi.Data;
using ProductsApi.Data.Entities;
using ProductsApi.Models;
using ProductsApi.Service;
using System.Text.Json;

namespace ProductsApi.Controllers
{
    // [Route("api/[controller]")]
    [Route("api/products")]
    // [Route("api/v{version:apiVersion}/products")]
    [Asp.Versioning.ApiVersion("1")]
    [Asp.Versioning.AdvertiseApiVersions("1")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<ProductsController> logger;
        private readonly IDistributedCache distributedCache;

        private const string LimitedStockProductsKey = "LSPC";
        private const string OverstockedProductsKey = "OSPK";

        public ProductsController(IProductService productService,
            IMapper mapper,
            IMemoryCache memoryCache,
            ILogger<ProductsController> logger,
            IDistributedCache distributedCache)
        {
            _productService = productService;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.distributedCache = distributedCache;
        }


        [HttpGet]
        [Produces("application/vnd.example.v1+json")]
        public async Task<IActionResult> GetProducts(int? categoryId)
        {
            var products = await _productService.GetProductsAsync();

            var productsToReturn = mapper.Map<IEnumerable<ProductModel>>(products);
            // var trimmedProducts = mapper.Map<IEnumerable<ProductTrimmed>>(products);
            return Ok(productsToReturn);
            // TODO: add mappings and use the Model if needed
        }


        // GET: api/products/limitedstock
        [HttpGet]
        [Route("limitedstock")]
        [Produces(typeof(Product[]))]
        [ResponseCache(Duration = 5, // Cache-Control: max-age=5
          Location = ResponseCacheLocation.Any, // Cache-Control: public
          VaryByHeader = "User-Agent" // Vary: User-Agent

        )]
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
                    SlidingExpiration = TimeSpan.FromSeconds(120),
                    Size = cachedValue?.Length
                };

                memoryCache.Set(LimitedStockProductsKey, cachedValue, cacheEntryOptions);
            }
            MemoryCacheStatistics? stats = memoryCache.GetCurrentStatistics();
            logger.LogInformation($"Memory cache. Total hits: {stats?
                  .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");
            return cachedValue ?? Enumerable.Empty<Product>();
        }

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

            byte[]? cachedValueBytes = JsonSerializer.SerializeToUtf8Bytes(cachedValue);

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

        // GET: api/Products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productModel = mapper.Map<ProductModel>(product);
            return Ok(productModel);
        }


        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (!await _productService.ProductExistsAsync(id))
            {
                return NotFound();
            }

            //redundant
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            await _productService.UpdateProductAsync(product);

            return Ok(product);

        }


        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductModel productModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var productToAdd = mapper.Map<Product>(productModel);
            var createdProduct = await _productService.AddProductAsync(productToAdd);

            return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
        }


        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpHead("{id}")]
        public async Task<ActionResult> CheckIfExists(int id)
        {
            var existingProduct = await _productService.ProductExistsAsync(id);

            if (!existingProduct)
            {
                return NotFound();
            }
            return Ok("ceva");
        }

    }
}

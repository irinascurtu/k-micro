# adding the stocks endpoint

- Implement a new method in the ProductRepostitory that retrieves the stocks for a list of products

```csharp

 public async Task<IEnumerable<ProductStock>> GetProductStocksAsync(List<int> productIds)
 {
     var stocks = await _context.Products.Where(p => productIds.Contains(p.Id)).Select(x => new ProductStock
     {
         Stock = x.Stock,
         ProductId = x.Id
     }).ToListAsync();
     return stocks;
 }
```

- Expose the method in the interface.
- make the method available in the `ProductService` too.


```csharp
public async Task<IEnumerable<ProductStock>> GetProductsStocksAsync(List<int> productsIds)
{
    return await _productRepository.GetProductStocksAsync(productsIds);
}
```

## Add Update Stocks method

- add a new method in the Repository that knows how to update stocks for some specific products

  ```csharp
     public async Task<IEnumerable<ProductStock>> UpdateProductStocks(Dictionary<int, int> productStocks)
     {
         foreach (var kvp in productStocks)
         {
             var product = await _context.Products.FindAsync(kvp.Key);
             if (product != null)
             {
                 product.Stock = kvp.Value;
                 _context.Entry(product).State = EntityState.Modified;
             }
         }
         await _context.SaveChangesAsync();
         var updatedStocks = await _context.Products
             .Where(p => productStocks.ContainsKey(p.Id))
             .Select(x => new ProductStock
             {
                 Stock = x.Stock,
                 ProductId = x.Id
             })
             .ToListAsync();
         return updatedStocks;
     }
  ```

## Adding the Stocks endpoint

- create a controller named `Stocks` that uses the two methods created

```csharp
  private readonly IProductService productService;

  public StocksController(IProductService productService)
  {
      this.productService = productService;
  }

```

- Implement `GetProductsStocks`
       
```csharp
[HttpGet]
 public async Task<IActionResult> GetProductsStocks([FromQuery] List<int> productIds)
 {
     var products = await productService.GetProductsStocksAsync(productIds);
     return Ok(products);
     ///TODO: add mappings and use the Model
     ///
 }

```

## Call the endpoints to see if they work as intended
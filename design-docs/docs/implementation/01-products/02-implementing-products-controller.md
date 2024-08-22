# Step-by-Step Guide to Implementing `ProductsController`

This guide will walk you through the process of implementing a `ProductsController` class in ASP.NET Core, which handles CRUD operations for products using an `IProductService` interface.

## Define the `IProductService` Interface

Before implementing the `ProductsController`, ensure that the `IProductService` interface is defined in your project. This interface should declare the methods used in the controller, such as `GetProductsAsync`, `GetProductAsync`, `UpdateProductAsync`, `AddProductAsync`, and `DeleteProductAsync`.

<details>
<summary>Click to view an example of the `IProductService` interface.</summary>

```csharp
public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product> GetProductAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
}
```

</details>

## Register the IProductService in Dependency Injection

```csharp
  services.AddScoped<IProductService, ProductService>();

```
## Create the ProductsController Class
Create a new controller named `ProductsController.cs` in your Controllers folder. This class should be decorated with the [Route] and [ApiController] attributes to define it as a controller and to route HTTP requests to its methods.

<details>
<summary>Click to view the `ProductsController` class declaration.</summary>

```csharp

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
}

```

</details>


# Step-by-Step Guide to Implementing `ProductsController`

This guide will walk you through the process of implementing a `ProductsController` class in ASP.NET Core, which handles CRUD operations for products using an `IProductService` interface.

## Step 1: Define the `IProductService` Interface
Before implementing the `ProductsController`, ensure that the `IProductService` interface is defined in your project. This interface should declare the methods used in the controller, such as `GetProductsAsync`, `GetProductAsync`, `UpdateProductAsync`, `AddProductAsync`, and `DeleteProductAsync`.
<details>
<summary>Click to view the `ProductsController` class declaration.</summary>
```csharp
public interface IProductService
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product> GetProductAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
}
```
</details>

## Step 2: Create the ProductsController Class
Create a new class named ProductsController.cs in your Controllers folder. This class should be decorated with the [Route] and [ApiController] attributes to define it as a controller and to route HTTP requests to its methods.

<details>
<summary>Click to view the `ProductsController` class declaration.</summary>

```csharp

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
}
```
</details>

##  Implement the GetProducts Method
The GetProducts method is an HTTP GET endpoint that retrieves a list of products. It calls the GetProductsAsync method from the IProductService and returns the products in an Ok response.

<details>
<summary>Click to view the code </summary>

```csharp

[HttpGet]
public async Task<IActionResult> GetProducts()
{
    var products = await _productService.GetProductsAsync();
    return Ok(products);
    // TODO: add mappings and use the Model if needed
}
```
</details>

## Implement the GetProduct Method
The GetProduct method is an HTTP GET endpoint that retrieves a single product by its ID. If the product is not found, it returns a NotFound response.

<details>
<summary>Click to view the implementation of the `GetProduct` method.</summary>

```csharp

// GET: api/Products/{id}
[HttpGet("{id}")]
public async Task<ActionResult<Product>> GetProduct(int id)
{
    var product = await _productService.GetProductAsync(id);
    if (product == null)
    {
        return NotFound();
    }

    return Ok(product);
}
```
</details>

## Implement the PutProduct Method

The `PutProduct` method is an HTTP PUT endpoint that updates an existing product. It checks if the product ID matches the route ID, and if not, returns a `BadRequest` response. It also handles exceptions related to product existence.


<details>
<summary>Click to view the implementation of the `PutProduct` method.</summary>

```csharp
// PUT: api/Products/{id}
[HttpPut("{id}")]
public async Task<IActionResult> PutProduct(int id, Product product)
{
    if (id != product.Id)
    {
        return BadRequest();
    }

    try
    {
        await _productService.UpdateProductAsync(product);
    }
    catch
    {
        if (!await _productService.ProductExistsAsync(id))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }

    return NoContent();
}
```
</details>

## Implement the PostProduct Method

The `PostProduct` method is an HTTP POST endpoint that creates a new product. It calls the `AddProductAsync` method from the `IProductService` and returns a `CreatedAtAction` response.

<details>
<summary>Click to view the implementation of the `PostProduct` method.</summary>

```csharp

// POST: api/Products
[HttpPost]
public async Task<ActionResult<Product>> PostProduct(Product product)
{
    var createdProduct = await _productService.AddProductAsync(product);
    return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
}

```
</details>

## Implement the DeleteProduct Method
The `DeleteProduct` method is an HTTP DELETE endpoint that deletes a product by its ID. If the product is not found, it returns a `NotFound` response.

<details>
<summary>Click to view the implementation of the `DeleteProduct` method.</summary>

```csharp

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

```
</details>


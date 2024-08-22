## Http methods
Using the right 
### GET

Get all the items in the list.
```c#
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
            ///TODO: add mappings and use the Model
        }
```

Get the items that correspond to a specific ID
```c#
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
### POST

Create a new resource

```c#
// POST: api/Products
[HttpPost]
public async Task<ActionResult<Product>> PostProduct(Product product)
{
    var createdProduct = await _productService.AddProductAsync(product);
    return CreatedAtAction("GetProduct", new { id = createdProduct.Id }, createdProduct);
}
```


### PUT
Update an existing resource
```c#
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

Now let's update the method to be a bit more clear.
<details>
  <summary>POST</summary>

```csharp

```
</details>



### DELETE
```c#
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


## Http status codes




## Http headers

### Accept:

### Content-Type:

## Best Practices
### Pagination for large data sets
### Filtering




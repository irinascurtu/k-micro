
This guide provides a step-by-step process for creating the `OrdersController` in an ASP.NET Core Web API project. The controller is responsible for handling CRUD operations for orders and interacting with services such as order management and product stock verification.

## Step 1: Create the Controller Class

1. **Namespace and Class Definition**:
    - Define the namespace for the controller.
    - Create the `OrdersController` class and inherit from `ControllerBase`.
    - Apply the `[ApiController]` and `[Route("api/[controller]")]` attributes to the class.

<details>
      <summary>Code</summary>

    ```csharp
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using OrdersApi.Data.Domain;
    using OrdersApi.Models;
    using OrdersApi.Service.Clients;
    using OrdersApi.Services;

    namespace OrdersApi.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class OrdersController : ControllerBase
        {
        }
    }

    ```
 </details>

## Step 2: Define Dependencies via Constructor Injection

2. **Inject Required Services**:
    - Inject `IOrderService`, `IProductStockServiceClient`, and `IMapper` via the constructor.
    - Assign these dependencies to private readonly fields.

<details>
      <summary>Code</summary>

    ```csharp
        private readonly IOrderService _orderService;
        private readonly IProductStockServiceClient _productStockServiceClient;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper)
        {
            _orderService = orderService;
            _productStockServiceClient = productStockServiceClient;
            _mapper = mapper;
        }
    ```

</details>

## Step 3: Implement CRUD Operations

3. **Implement `GET: api/Orders`**:
    - Define a method `GetOrders` that retrieves all orders asynchronously using `_orderService`.
    - Return the list of orders with an `Ok` response.

<details>
      <summary>Code</summary>

    ```csharp
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }
    ```

</details>

4. **Implement `GET: api/Orders/{id}`**:
    - Define a method `GetOrder` that retrieves a specific order by its ID.
    - If the order is not found, return `NotFound`.
    - Otherwise, return the order with an `Ok` response.

<details>
      <summary>Code</summary>

    ```csharp
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    ```

</details>

5. **Implement `PUT: api/Orders/{id}`**:
    - Define a method `PutOrder` to update an existing order.
    - Check if the ID in the URL matches the order ID in the body; if not, return `BadRequest`.
    - Attempt to update the order via `_orderService`. If the order does not exist, return `NotFound`.
    - On success, return `NoContent`.

<details>
      <summary>Code</summary>

    ```csharp
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            try
            {
                await _orderService.UpdateOrderAsync(order);
            }
            catch
            {
                if (!await _orderService.OrderExistsAsync(id))
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

6. **Implement `POST: api/Orders`**:
    - Define a method `PostOrder` to create a new order.
    - Use `_productStockServiceClient` to check the stock of the ordered items.
    - If stock is insufficient, handle the error appropriately.
    - Map the `OrderModel` to `Order` using `_mapper` and save it via `_orderService`.
    - Return the created order with a `CreatedAtAction` response.

<details>
      <summary>Code</summary>

    ```csharp
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            var stocks = await _productStockServiceClient.GetStock(
                model.OrderItems.Select(p => p.ProductId).ToList());

            // Verify if all products have stock
            if (!await VerifyStocks(stocks, model.OrderItems))
            {
                // Add model state error: "Sorry, we can't process your order, we don't have enough stock for item."
                ModelState.AddModelError("StockError", "Sorry, we can't process your order due to insufficient stock.");
                return BadRequest(ModelState);
            }

            var orderToAdd = _mapper.Map<Order>(model);
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
            // Diminish stock

            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }
    ```

</details>

7. **Implement `DELETE: api/Orders/{id}`**:
    - Define a method `DeleteOrder` to remove an order by ID.
    - If the order is not found, return `NotFound`.
    - Otherwise, delete the order and return `NoContent`.

<details>
      <summary>Code</summary>

    ```csharp
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    ```

</details>

## Step 4: Implement Helper Methods

8. **Verify Product Stocks**:
    - Define a private method `VerifyStocks` to check if the stock is sufficient for all ordered items.
    - Return `true` if all items have sufficient stock; otherwise, return `false`.

<details>
      <summary>Code</summary>

    ```csharp
        private async Task<bool> VerifyStocks(List<ProductStock> stocks, List<OrderItemModel> orderItems)
        {
            foreach (var item in orderItems)
            {
                var stock = stocks.FirstOrDefault(s => s.ProductId == item.ProductId);
                if (stock == null || stock.Stock < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }
    ```

</details>

## Step 5: Finalize and Test the Controller

9. **Final Steps**:
    - Ensure all dependencies are correctly registered in the `Startup` or `Program` class.
    - Test the controller methods using tools like Postman or Swagger UI.
    - Ensure that error handling and stock verification are functioning as expected.

 <details>
      <summary>Code</summary>

    ```csharp
  

    ```

</details>

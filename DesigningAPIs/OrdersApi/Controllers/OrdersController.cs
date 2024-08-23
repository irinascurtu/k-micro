using AutoMapper;
using Microsoft.AspNetCore.Http;
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

            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, null);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id, 
            [FromQuery] string? culture, 
            [FromHeader] string? culture2)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var orderModel = _mapper.Map<OrderModel>(order);
            return Ok(orderModel);
        }

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
    }
}

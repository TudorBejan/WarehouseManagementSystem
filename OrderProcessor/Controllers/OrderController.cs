using Commons.Model.Order;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace OrderProcessor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class OrderController : ControllerBase
    {
        private readonly OrderHandler _handler;
        private readonly IOrdersHistoryRepository _ordersHistoryRepo;

        public OrderController(OrderHandler handler, IOrdersHistoryRepository ordersHistoryRepository)
        {
            _handler = handler;
            _ordersHistoryRepo = ordersHistoryRepository;
        }


        [HttpPost("/orders")]
        [ProducesResponseType<List<OrderProcessingResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerRequestExample(typeof(Order), typeof(ListOrderExample))]
        public IActionResult PostOrders(List<Order> orders)
        {
            if (orders == null || orders.Count == 0) 
            { 
                return BadRequest("The orders list should not be empty."); 
            }

            return Ok(_handler.ProcessOrders(orders));
        }

        [HttpGet("/ordersHistory")]
        [ProducesResponseType<List<OrderHistoryResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetOrdersHistory()
        {
            var orderHistory = _ordersHistoryRepo.GetOrdersHistory();

            if (orderHistory == null || orderHistory.Count == 0) {
                return NotFound();
            }

            var result = orderHistory
                .Select(o => new OrderHistoryResponse()
                    {
                        Id = o.Id, 
                        Order = JsonSerializer.Deserialize<Order>(o.Order), 
                        OrderStatus = o.OrderStatus,
                        ProcessedAt = o.ProcessedAt
                    })
                .ToList();

            return Ok(result);
        }
    }
}

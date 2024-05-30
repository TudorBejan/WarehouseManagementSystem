using System.Text.Json;

namespace Commons.Model.Order
{
    public class OrderHistory
    {
        public string Id { get; set; }
        public string Order { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime ProcessedAt { get; set; }

        public OrderHistory()
        {

        }
        public OrderHistory(string id, Order order, OrderStatus status)
        {
            Id = id;
            Order = JsonSerializer.Serialize(order);
            OrderStatus = status;
            ProcessedAt = DateTime.Now;
        }
    }
}

namespace Commons.Model.Order
{
    public class OrderHistoryResponse
    {
        public string Id { get; set; }
        public Order Order { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime ProcessedAt { get; set; }

    }
}

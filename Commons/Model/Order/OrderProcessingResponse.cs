namespace Commons.Model.Order
{
    public class OrderProcessingResponse
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string ReasonForFailure { get; set; }

        public OrderProcessingResponse(int orderId, OrderStatus status, string message)
        {
            OrderId = orderId;
            OrderStatus = status;
            ReasonForFailure = message;
        }
    }
}

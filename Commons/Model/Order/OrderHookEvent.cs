using Commons.Model.Product;

namespace Commons.Model.Order
{
    public class OrderHookEvent
    {
        public string Id { get; set; }
        public Order Order { get; set; }
        public ProductLocation ProductLocation { get; set; }

        public OrderHookEvent() { }

        public OrderHookEvent(Order order, ProductLocation productLocation)
        {
            Id = Guid.NewGuid().ToString();
            Order = order;
            ProductLocation = productLocation;
        }
    }
}

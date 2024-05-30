using Commons.Model.Product;

namespace Commons.Model.Order
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public ProductType ProductType { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }

    }
}

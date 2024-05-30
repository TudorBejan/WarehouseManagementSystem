namespace Commons.Model.Product
{
    public class ProductLocation
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseFloor { get; set; }
        public string WarehouseArea { get; set; }
        public int WarehouseSection { get; set; }
    }
}

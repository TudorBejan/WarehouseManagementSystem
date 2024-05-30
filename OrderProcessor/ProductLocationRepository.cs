using Commons.Model.Product;

namespace OrderProcessor
{
    public interface IProductLocationRepository 
    {
        ProductLocation GetProductLocation(int productId);
    }
    public class ProductLocationRepository : IProductLocationRepository
    {

        private readonly WarehouseDbContext _context;

        public ProductLocationRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public ProductLocation GetProductLocation(int productId)
        {
            return _context.ProductLocations.SingleOrDefault(x => x.ProductId == productId);
        }
    }
}

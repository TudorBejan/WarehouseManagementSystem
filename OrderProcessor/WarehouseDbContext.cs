using Commons.Model.Order;
using Commons.Model.Product;
using Microsoft.EntityFrameworkCore;

namespace OrderProcessor
{
    public class WarehouseDbContext : DbContext
    {
        public DbSet<OrderHistory> OrdersHistory { get; set; }
        public DbSet<ProductLocation> ProductLocations { get; set; }

        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
            : base(options)
        {
        }
    }
}

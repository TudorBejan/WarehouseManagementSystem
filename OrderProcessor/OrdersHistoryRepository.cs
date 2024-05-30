using Commons.Model.Order;

namespace OrderProcessor
{
    public interface IOrdersHistoryRepository
    {
        void InsertOrderHistory(OrderHistory orderHistory);
        List<OrderHistory> GetOrdersHistory();
    }

    public class OrdersHistoryRepository : IOrdersHistoryRepository
    {
        private readonly WarehouseDbContext _context;

        public OrdersHistoryRepository(WarehouseDbContext context)
        {
            _context = context;
        }

        public void InsertOrderHistory(OrderHistory orderHistory)
        {
            _context.OrdersHistory.Add(orderHistory);
            _context.SaveChanges();
        }

        public List<OrderHistory> GetOrdersHistory()
        {
            return _context.OrdersHistory.ToList();
        }
    }
}

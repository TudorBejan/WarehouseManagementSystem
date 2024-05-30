using Commons.Model;
using Commons.Model.Product;

namespace WebHookDispatcher
{
    public class SubscriptionRepository
    {
        private readonly SubscriptionDbContext _context;

        public SubscriptionRepository(SubscriptionDbContext context)
        {
            _context = context;
        }

        public void AddItem(Subscription sub)
        {
            _context.Subscriptions.Add(sub);
            _context.SaveChanges();
        }

        public List<Subscription> GetSubscriptionsByTopic(ProductType topic)
        {
            return _context.Subscriptions.Where(sub => sub.Topic == topic).ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Commons.Model;

namespace WebHookDispatcher
{
    public class SubscriptionDbContext : DbContext
    {
        public DbSet<Subscription> Subscriptions { get; set; }

        public SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options)
            : base(options)
        {
        }
    }
}

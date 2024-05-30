using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Commons.Model;
using Commons.Model.Product;
using WebHookDispatcher;
using Commons;

var services = new ServiceCollection();

services.AddScoped<SubscriptionRepository>();
services.AddScoped<WHCallbackCaller>();
services.AddScoped<WebHookHandler>();
services.AddScoped<WebHookEventConsumer>();

services.AddDbContext<SubscriptionDbContext>(options =>
    options.UseInMemoryDatabase(MyConstants.SUBSCRIPTION_DB));

var serviceProvider = services.BuildServiceProvider();
PopulateSubscriptionDB(serviceProvider.GetRequiredService<SubscriptionDbContext>());

WebHookEventConsumer consumer = serviceProvider.GetRequiredService<WebHookEventConsumer>();
//start the consumer
consumer.Consume();


static void PopulateSubscriptionDB(SubscriptionDbContext context)
{
    context.Add(new Subscription() { Id = 1, Topic = ProductType.TV, CallbackURL = "http://localhost:5030/webhook/order/new" });
    context.Add(new Subscription() { Id = 2, Topic = ProductType.Smartphone, CallbackURL = "http://localhost:5031/webhook/order/new" });
    context.Add(new Subscription() { Id = 3, Topic = ProductType.Tablet, CallbackURL = "http://localhost:5031/webhook/order/new" });
    context.SaveChanges();
}

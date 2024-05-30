using Commons.Model;
using Commons.Model.Order;

namespace WebHookDispatcher
{
    public class WebHookHandler
    {
        private readonly WHCallbackCaller _callbackCaller;
        private readonly SubscriptionRepository _subscriptionRepository;

        public WebHookHandler(SubscriptionRepository repository, WHCallbackCaller caller)
        {
            _subscriptionRepository = repository;
            _callbackCaller = caller;
        }

        public void HandleWebhook(OrderHookEvent orderHookEvent)
        {
            List<Subscription> subscriptions = _subscriptionRepository.GetSubscriptionsByTopic(orderHookEvent.Order.ProductType);
            foreach (Subscription subscription in subscriptions)
            {
                try
                {
                    Task task = _callbackCaller.InsertWebHookAsync(subscription.CallbackURL, orderHookEvent);
                    task.Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not push webhook to " + subscription.CallbackURL + " Reason: " + ex.Message);
                }

            }
        }
    }
}

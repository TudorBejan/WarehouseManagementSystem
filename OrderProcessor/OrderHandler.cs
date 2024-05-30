using Commons.Model.Order;
using Commons.Model.Product;

namespace OrderProcessor
{
    public class OrderHandler
    {

        private readonly IOrdersHistoryRepository _orderHistoryRepo;
        private readonly IProductLocationRepository _productLocationRepo;
        private readonly IWebHookEventPublisher _webHookEventPublisher;

        public OrderHandler(
            IOrdersHistoryRepository orderHistorytRepo, 
            IProductLocationRepository productLocationRepository,
            IWebHookEventPublisher eventPublisher)
        {
            _orderHistoryRepo = orderHistorytRepo;
            _productLocationRepo = productLocationRepository;
            _webHookEventPublisher = eventPublisher;
        }

        public List<OrderProcessingResponse> ProcessOrders(List<Order> orders)
        {
            List<OrderProcessingResponse> orderResponses = new();
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    try
                    {
                        //find the product location
                        ProductLocation productLocation = _productLocationRepo.GetProductLocation(order.ProductId);
                        if (productLocation == null)
                        {
                            //store the order as failed
                            _orderHistoryRepo.InsertOrderHistory(new OrderHistory(order.Id + "_FAILED", order, OrderStatus.Failed));
                            orderResponses.Add(new OrderProcessingResponse(order.Id, OrderStatus.Failed, "No product location found"));
                        }
                        else
                        {
                            //publish the webhook event on the queue
                            OrderHookEvent orderHookEvent = new OrderHookEvent(order, productLocation);
                            _webHookEventPublisher.PublishWebhookEvent(orderHookEvent);
                            //store the order as successed
                            _orderHistoryRepo.InsertOrderHistory(new OrderHistory(orderHookEvent.Id, order, OrderStatus.Success));
                            orderResponses.Add(new OrderProcessingResponse(order.Id, OrderStatus.Success, null));
                        }
                    }
                    catch (Exception ex)
                    {
                        //store the order as failed
                        _orderHistoryRepo.InsertOrderHistory(new OrderHistory(order.Id + "_FAILED", order, OrderStatus.Failed));
                        orderResponses.Add(new OrderProcessingResponse(order.Id, OrderStatus.Failed, "Could not process order: " + ex.Message));
                    }
                }
                return orderResponses;
            }
            else
            {
                return orderResponses;
            }
        }
    }
}

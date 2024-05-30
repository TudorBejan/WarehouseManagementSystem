using Commons.Model.Order;
using Commons.Model.Product;
using NSubstitute;

namespace OrderProcessor.UnitTests
{
    public class OrderHandlerTests
    {
        private readonly IOrdersHistoryRepository _orderHistoryRepo = Substitute.For<IOrdersHistoryRepository>();
        private readonly IProductLocationRepository _productLocationRepo = Substitute.For<IProductLocationRepository>();
        private readonly IWebHookEventPublisher _webHookEventPublisher = Substitute.For<IWebHookEventPublisher>();

        private OrderHandler _orderHandler;

        public OrderHandlerTests()
        {
            _orderHandler = new OrderHandler(_orderHistoryRepo, _productLocationRepo, _webHookEventPublisher);
        }

        [Fact]
        public void ProcessOrders_OrderListNull_ExpectEmptyResponseList()
        {
            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(null);

            Assert.NotNull(response);
            Assert.False(response.Any());

            _productLocationRepo.DidNotReceive().GetProductLocation(Arg.Any<int>());
            _orderHistoryRepo.DidNotReceive().InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.DidNotReceive().PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        [Fact]
        public void ProcessOrders_OrderListEmpty_ExpectEmptyResponseList()
        {
            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(new List<Order>());

            Assert.NotNull(response);
            Assert.False(response.Any());

            _productLocationRepo.DidNotReceive().GetProductLocation(Arg.Any<int>());
            _orderHistoryRepo.DidNotReceive().InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.DidNotReceive().PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        [Fact]
        public void ProcessOrders_OneOrderNotInProductLocation_ExpectFailedOrder()
        {
            Order badOrder = GenerateOrders()[2];

            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(new List<Order>() { badOrder });

            Assert.NotNull(response);
            Assert.Single(response);
            AssertOrderResponse(badOrder.Id, OrderStatus.Failed, "No product location found", response[0]);

            _productLocationRepo.Received().GetProductLocation(badOrder.ProductId);
            _orderHistoryRepo.Received().InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.DidNotReceive().PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        [Fact]
        public void ProcessOrders_OneOrderInProductLocation_ExpectSuccessOrder()
        {
            Order goodOrder = GenerateOrders()[0];

            ProductLocation pl1 = GenerateProductLocations()[0];

            _productLocationRepo.GetProductLocation(goodOrder.ProductId).Returns(pl1);

            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(new List<Order>() { goodOrder });

            Assert.NotNull(response);
            Assert.Single(response);
            AssertOrderResponse(goodOrder.Id, OrderStatus.Success, null, response[0]);

            _orderHistoryRepo.Received().InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.Received().PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        [Fact]
        public void ProcessOrders_TwoOdersOneGoodOneBad_ExpectOneSuccessOneFailedOrder()
        {
            Order goodOrder = GenerateOrders()[0];
            Order badOrder = GenerateOrders()[2];

            ProductLocation pl1 = GenerateProductLocations()[0];

            _productLocationRepo.GetProductLocation(goodOrder.ProductId).Returns(pl1);

            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(new List<Order>() { goodOrder, badOrder });

            Assert.NotNull(response);
            Assert.Equal(2, response.Count());

            AssertOrderResponse(goodOrder.Id, OrderStatus.Success, null, response[0]);
            AssertOrderResponse(badOrder.Id, OrderStatus.Failed, "No product location found", response[1]);

            _orderHistoryRepo.Received(2).InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.Received(1).PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        [Fact]
        public void ProcessOrders_TwoGoodOders_ExpectTwoSuccessdOrders()
        {
            Order goodOrder1 = GenerateOrders()[0];
            Order goodOrder2 = GenerateOrders()[1];

            ProductLocation pl1 = GenerateProductLocations()[0];
            ProductLocation pl2 = GenerateProductLocations()[1];

            _productLocationRepo.GetProductLocation(goodOrder1.ProductId).Returns(pl1);
            _productLocationRepo.GetProductLocation(goodOrder2.ProductId).Returns(pl2);

            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(new List<Order>() { goodOrder1, goodOrder2 });

            Assert.NotNull(response);
            Assert.Equal(2, response.Count());

            AssertOrderResponse(goodOrder1.Id, OrderStatus.Success, null, response[0]);
            AssertOrderResponse(goodOrder2.Id, OrderStatus.Success, null, response[1]);

            _orderHistoryRepo.Received(2).InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.Received(2).PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        [Fact]
        public void ProcessOrders_TwoGoodOders_ExceptionAtSecondOne_ExpectOneSuccessOneFailedOrder()
        {
            Order goodOrder1 = GenerateOrders()[0];
            Order goodOrder2 = GenerateOrders()[1];

            ProductLocation pl1 = GenerateProductLocations()[0];
            ProductLocation pl2 = GenerateProductLocations()[1];

            _productLocationRepo.GetProductLocation(goodOrder1.ProductId).Returns(pl1);
            _productLocationRepo.GetProductLocation(goodOrder2.ProductId).Returns(pl2);

            //now when we process the second order, the message broker throws an exception
            _webHookEventPublisher
                .When(x => x.PublishWebhookEvent(Arg.Is<OrderHookEvent>(ohe => ohe.Order == goodOrder2)))
                .Do(x => { throw new Exception("Intended exception"); });

            List<OrderProcessingResponse> response = _orderHandler.ProcessOrders(new List<Order>() { goodOrder1, goodOrder2 });

            Assert.NotNull(response);
            Assert.Equal(2, response.Count());

            AssertOrderResponse(goodOrder1.Id, OrderStatus.Success, null, response[0]);
            AssertOrderResponse(goodOrder2.Id, OrderStatus.Failed, "Could not process order: Intended exception", response[1]);

            _orderHistoryRepo.Received(2).InsertOrderHistory(Arg.Any<OrderHistory>());
            _webHookEventPublisher.Received(2).PublishWebhookEvent(Arg.Any<OrderHookEvent>());
        }

        private void AssertOrderResponse(int expectedOrderId, OrderStatus expectedOrderStatus, String expectedReasonForFailure, 
                                        OrderProcessingResponse response)
        {
            Assert.Equal(expectedOrderId, response.OrderId);
            Assert.Equal(expectedOrderStatus, response.OrderStatus);
            Assert.Equal(expectedReasonForFailure, response.ReasonForFailure);
        }

        private List<Order> GenerateOrders()
        {
            Order goodOrder1 = new Order()
            {
                Id = 100,
                ProductId = 1001,
                ProductName = "Test Product",
                ProductType = ProductType.TV,
                Quantity = 1
            };

            Order goodOrder2 = new Order()
            {
                Id = 102,
                ProductId = 2001,
                ProductName = "Test Product 2",
                ProductType = ProductType.Smartphone,
                Quantity = 1
            };
            Order badOrder = new Order()
            {
                Id = 14,
                ProductId = 101,
                ProductName = "Test Product 2",
                ProductType = ProductType.Tablet,
                Quantity = 2
            };
            List<Order> orders = new List<Order>() { goodOrder1, goodOrder2, badOrder };
            return orders;
        }

        private List<ProductLocation> GenerateProductLocations()
        {
            ProductLocation pl1 = new ProductLocation()
            {
                Id = 1,
                ProductId = 1001,
                WarehouseArea = "A",
                WarehouseFloor = 0,
                WarehouseSection = 1
            };
            ProductLocation pl2 = new ProductLocation()
            {
                Id = 1,
                ProductId = 2001,
                WarehouseArea = "A",
                WarehouseFloor = 0,
                WarehouseSection = 2
            };
            List<ProductLocation> productLocations = new List<ProductLocation>() { pl1, pl2 };
            return productLocations;
        }
    }
}
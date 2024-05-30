using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Commons.Model.Order;
using Commons;

namespace WebHookDispatcher
{
    internal class WebHookEventConsumer
    {
        private readonly WebHookHandler _webHookHandler;

        public WebHookEventConsumer(WebHookHandler webHookHandler)
        {
            _webHookHandler = webHookHandler;
        }

        public void Consume()
        {
            var factory = new ConnectionFactory { HostName = MyConstants.BROKER_URL };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: MyConstants.QUEUE_NAME,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine("Waiting for webhook events:");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceiveEvent;

            channel.BasicConsume(queue: MyConstants.QUEUE_NAME,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private void ReceiveEvent(object? model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            OrderHookEvent? orderHookEvent = JsonSerializer.Deserialize<OrderHookEvent>(message);

            Console.WriteLine($"Received webhook event that should be pushed " +
                $"{JsonSerializer.Serialize(orderHookEvent, new JsonSerializerOptions { WriteIndented = true })}");

            _webHookHandler.HandleWebhook(orderHookEvent);
        }
    }
}

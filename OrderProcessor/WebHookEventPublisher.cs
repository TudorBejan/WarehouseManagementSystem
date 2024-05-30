using Commons;
using Commons.Model.Order;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderProcessor
{
    public interface IWebHookEventPublisher
    {
        void PublishWebhookEvent(OrderHookEvent orderHookEvent);
    }
    public class WebHookEventPublisher : IWebHookEventPublisher
    {
        public void PublishWebhookEvent(OrderHookEvent orderHookEvent)
        {
            var factory = new ConnectionFactory { HostName = MyConstants.BROKER_URL };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: MyConstants.QUEUE_NAME,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var message = JsonSerializer.Serialize(orderHookEvent);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                                 routingKey: MyConstants.QUEUE_NAME,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"Webhook event sent: " +
                $"{JsonSerializer.Serialize(orderHookEvent, new JsonSerializerOptions { WriteIndented = true })}");
        }

    }
}

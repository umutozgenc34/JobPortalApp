using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Shared.Rabbit;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class NotificationPublisher
{
    private readonly RabbitMQClient _rabbitMQClient;

    public NotificationPublisher(RabbitMQClient rabbitMQClient)
    {
        _rabbitMQClient = rabbitMQClient;
    }

    public void Publish(NotificationMessage message)
    {
        var channel = _rabbitMQClient.GetChannel();

        channel.QueueDeclare(queue: "notifications", durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(exchange: "",
                             routingKey: "notifications",
                             basicProperties: null,
                             body: body);
    }
}
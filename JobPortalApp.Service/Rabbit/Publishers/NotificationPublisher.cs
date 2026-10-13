using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Shared.Rabbit;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace JobPortalApp.Service.Rabbit.Publishers;

public sealed class NotificationPublisher(RabbitMQClient rabbitMQClient, ILogger<NotificationPublisher> logger) : IDisposable
{
    private readonly object _sync = new();
    private IModel? _channel;

    public void Publish(NotificationMessage message)
    {
        try
        {
            lock (_sync)
            {
                if (_channel is null || _channel.IsClosed)
                {
                    _channel?.Dispose();
                    _channel = rabbitMQClient.CreateChannel();
                    _channel.QueueDeclare(queue: RabbitQueues.Notifications, durable: true, exclusive: false, autoDelete: false);
                }

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                _channel.BasicPublish(exchange: "",
                                      routingKey: RabbitQueues.Notifications,
                                      basicProperties: properties,
                                      body: body);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Notification could not be published to RabbitMQ. Title: {Title}", message.Title);
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _channel?.Dispose();
            _channel = null;
        }
    }
}

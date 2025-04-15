using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Shared.Rabbit;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace JobPortalApp.Service.Rabbit.Consumers;

public class NotificationConsumerBackgroundService : BackgroundService
{
    private readonly RabbitMQClient _rabbitMqClient;
    private IModel _channel;

    public NotificationConsumerBackgroundService(RabbitMQClient rabbitMqClient)
    {
        _rabbitMqClient = rabbitMqClient;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel = _rabbitMqClient.GetChannel();

        _channel.QueueDeclare(queue: "notifications",
                              durable: true,
                              exclusive: false,
                              autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            var payload = JsonSerializer.Deserialize<NotificationMessage>(messageJson);

            if (payload != null)
            {
                Console.WriteLine($"[Bildirim Geldi] Title: {payload.Title}, Message: {payload.Message}");

            }

            await Task.CompletedTask;
        };

        _channel.BasicConsume(queue: "notifications", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        base.Dispose();
    }
}
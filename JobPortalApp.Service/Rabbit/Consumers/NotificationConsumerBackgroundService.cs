using JobPortalApp.Model.Notifications.Entities;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Notifications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Shared.Rabbit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
    private IServiceScopeFactory _serviceScopeFactory; // scoped serviceleri resolve edebilmek için

    public NotificationConsumerBackgroundService(RabbitMQClient rabbitMqClient,IServiceScopeFactory serviceScopeFactory)
    {
        _rabbitMqClient = rabbitMqClient;
        _serviceScopeFactory = serviceScopeFactory;
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

                using var scope = _serviceScopeFactory.CreateScope();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var allUsers = userManager.Users.ToList();

                foreach (var user in allUsers)
                {
                    await notificationRepository.AddAsync(new Notification
                    {
                        Title = payload.Title,
                        Message = payload.Message,
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                await unitOfWork.SaveChangesAsync();
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
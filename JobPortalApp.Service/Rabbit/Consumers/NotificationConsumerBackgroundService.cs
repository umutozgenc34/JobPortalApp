using JobPortalApp.Model.Notifications.Entities;
using JobPortalApp.Model.Users.Entities;
using JobPortalApp.Repository.Notifications.Abstracts;
using JobPortalApp.Repository.UnitOfWorks.Abstracts;
using JobPortalApp.Service.Rabbit.Messages;
using JobPortalApp.Shared.Rabbit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace JobPortalApp.Service.Rabbit.Consumers;

public class NotificationConsumerBackgroundService(
    RabbitMQClient rabbitMqClient,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<NotificationConsumerBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(10);
    private IModel? _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _channel = rabbitMqClient.CreateChannel();
                _channel.QueueDeclare(queue: RabbitQueues.Notifications,
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false);
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += HandleMessageAsync;
                _channel.BasicConsume(queue: RabbitQueues.Notifications, autoAck: false, consumer: consumer);

                logger.LogInformation("Notification consumer is listening on queue {Queue}.", RabbitQueues.Notifications);
                return;
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(exception, "RabbitMQ is not reachable. Retrying in {Delay} seconds.", RetryDelay.TotalSeconds);
                _channel?.Dispose();
                _channel = null;
                await Task.Delay(RetryDelay, stoppingToken);
            }
        }
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        var channel = _channel!;
        try
        {
            var payload = JsonSerializer.Deserialize<NotificationMessage>(eventArgs.Body.Span);
            if (payload is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var userIds = await userManager.Users.Select(user => user.Id).ToListAsync();
                foreach (var userId in userIds)
                {
                    await notificationRepository.AddAsync(new Notification
                    {
                        Title = payload.Title,
                        Message = payload.Message,
                        UserId = userId
                    });
                }

                await unitOfWork.SaveChangesAsync();
                logger.LogInformation("Notification '{Title}' created for {Count} users.", payload.Title, userIds.Count);
            }

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Notification message could not be processed.");
            channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        base.Dispose();
    }
}

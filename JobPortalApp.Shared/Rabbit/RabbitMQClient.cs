using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace JobPortalApp.Shared.Rabbit;

public sealed class RabbitMQClient : IDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly object _sync = new();
    private IConnection? _connection;

    public RabbitMQClient(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMQ")!),
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true
        };
    }

    public IModel CreateChannel()
    {
        lock (_sync)
        {
            _connection ??= _factory.CreateConnection();
            return _connection.CreateModel();
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _connection?.Dispose();
            _connection = null;
        }
    }
}

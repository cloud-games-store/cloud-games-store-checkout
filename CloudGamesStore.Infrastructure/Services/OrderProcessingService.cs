using CloudGamesStore.Application.Interfaces;
using CloudGamesStore.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CloudGamesStore.Infrastructure.Services
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(
            IConfiguration configuration,
            ILogger<OrderProcessingService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ProcessPendingOrdersAsync(object message, string queueName)
        {
            var factory = new ConnectionFactory { HostName = _configuration["RabbitMq:Host"] };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName,
                                                    durable: false,
                                                    exclusive: false,
                                                    autoDelete: false,
                                                    arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "",
                                                    routingKey: queueName,
                                                    body: body);
        }

        
    }
}

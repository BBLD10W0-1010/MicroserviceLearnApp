using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;
using PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private IConnection? _connection;
        private IChannel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task ConnectAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQPort"] ?? "5672")
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;

            Console.WriteLine("--> Connected To message bus");
        }

        private Task RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"RABBITMQ --> Connection Shutdown: {e.ReplyText}");
            return Task.CompletedTask;
        }

        public async Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection OPEN");
                await SendMessageAsync(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection CLOSED");
            }
        }

        private async Task SendMessageAsync(string message)
        {
            var channel = _channel
                ?? throw new InvalidOperationException("RabbitMQ channel is not initialized");

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync<BasicProperties>(
                exchange: "trigger",
                routingKey: "",
                mandatory: false,
                basicProperties: new BasicProperties
                {
                    ContentType = "application/json"
                },
                body: body);

            System.Console.WriteLine($"We have sent {message}");
        }

        public void Dispose()
        {
            System.Console.WriteLine("MessageBus Disposed");
            if (_channel.IsOpen)
            {
                _channel.CloseAsync();
                _connection.CloseAsync();
            }
        }
    }
}
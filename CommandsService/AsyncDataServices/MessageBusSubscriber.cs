using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection? _connection;
        private IChannel? _channel;
        private QueueDeclareOk _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
        }

        private async Task InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = await _channel.QueueDeclareAsync();
            await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");
            System.Console.WriteLine("--> Listening on the message bus...");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
        }

        private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs @event)
        {
            System.Console.WriteLine("--> Connection shutdown");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeRabbitMQ();
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (ModuleHandle, ea) =>
            {
                System.Console.WriteLine("--> Event Received");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

                _eventProcessor.ProcessEvent(notificationMessage);
                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
            return;
        }

        public override void Dispose()
        {
            base.Dispose();

            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using password_usermanagement.Services;

namespace password_usermanagement.Queue
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly IRabbitMQConnection _connection;
        private IRoleService _roleService;
        private IModel _channel;
        private Action<string, string> _handler;
        private readonly string _queueName = "userRoleRequest";
        private readonly string _exchangeName = "authService";
        private readonly string _routingKey = "roleRequest";
        private IRabbitMQPublish _publisher;

        public RabbitMQListener(IRabbitMQConnection connection, IRabbitMQPublish publisher, IRoleService roleService)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _handler = HandleMessage;
            _roleService = roleService;
            _publisher = publisher;
        }
        private async void HandleMessage(string message, string concurrencyId)
        {
            Console.WriteLine($"Received message: {message} with concurrency ID: {concurrencyId}");

            var roles = await _roleService.GetRolesFromUser(message);
            var json = JsonConvert.SerializeObject(roles.ToString());
            await _publisher.Publish(json, "authService", "roleResponse", concurrencyId);

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(_queueName, _exchangeName, _routingKey);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject(jsonMessage);
                if (eventArgs.BasicProperties.Headers.TryGetValue("concurrency-id", out var concurrencyIdObj) &&
                    concurrencyIdObj is byte[] concurrencyIdBytes)
                {
                    var concurrencyId = Encoding.UTF8.GetString(concurrencyIdBytes);
                    _handler.Invoke(message.ToString(), concurrencyId);
                }
                else
                {
                    Console.WriteLine($"Received message: {message}");
                }
                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(_queueName, autoAck: false, consumer);
            await Task.Delay(-1);
        }
    }
}
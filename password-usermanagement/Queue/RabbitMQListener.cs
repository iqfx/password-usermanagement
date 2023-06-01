using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using password_usermanagement.Services;

namespace password_usermanagement.Queue
{
    public class RabbitMQListener : BackgroundService
    {
        private IModel _channel;
        private Action<string, string> _handler;
        private readonly string _queueName = "userRoleRequest";
        private readonly string _exchangeName = "authService";
        private readonly string _routingKey = "roleRequest";
        private IServiceProvider _provider;

        public RabbitMQListener(IServiceProvider provider)
        {
            _handler = HandleMessage;
            _provider = provider;
        }
        private async void HandleMessage(string message, string concurrencyId)
        {
            Console.WriteLine($"Received message: {message} with concurrency ID: {concurrencyId}");
            using (var scope = _provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<RoleService>();
                var publisher = scope.ServiceProvider.GetRequiredService<RabbitMQPublish>();

                var roles = await service.GetRolesFromUser(message);
                
                var json = JsonConvert.SerializeObject(roles.Select(p => new { p.RoleName }));
                await publisher.Publish(json, "authService", "userRoleRequestResponse", concurrencyId);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            stoppingToken.ThrowIfCancellationRequested();
            using (var scope = _provider.CreateScope())
            {
                var connection = scope.ServiceProvider.GetRequiredService<RabbitMQConnection>();
                _channel = connection.CreateModel();

                _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
                _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(_queueName, _exchangeName, _routingKey);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject(jsonMessage);
                    if (eventArgs.BasicProperties.CorrelationId != null)
                    {
                        // var concurrencyId = Encoding.UTF8.GetString(concurrencyIdBytes);
                        _handler.Invoke(message.ToString(), eventArgs.BasicProperties.CorrelationId);
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
}
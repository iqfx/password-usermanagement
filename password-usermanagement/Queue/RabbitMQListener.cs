using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace password_usermanagement.Queue
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly IRabbitMQConnection _connection;
        private IModel _channel;
        private Action<string> _handler;
        private string _queueName = "random";
        private string _exchangeName = "test";
        private string _routingKey = "test2";
        private void HandleMessage(string message)
        {
            Console.WriteLine($"Received message: {message}");
            // Perform additional processing
        }
        public RabbitMQListener(IRabbitMQConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _handler = HandleMessage;
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
                //await Task.Run(() => _handler.Invoke(message.ToString()));
                _handler.Invoke(message.ToString());
                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(_queueName, autoAck: false, consumer);
            await Task.Delay(-1);
        }
    }
}
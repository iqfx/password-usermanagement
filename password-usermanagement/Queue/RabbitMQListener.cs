using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace password_usermanagement.Queue
{
    public class RabbitMQListener : BackgroundService, IRabbitMQListener
    {
        private readonly IRabbitMQConnection _connection;
        private IModel _channel;
        private Action<string> _handler;
        private string _queueName = "random";
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

        public void init(string queueName, string exchangeName, string routingKey)
        {
            _queueName = queueName;
            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queueName, exchangeName, routingKey);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();  

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
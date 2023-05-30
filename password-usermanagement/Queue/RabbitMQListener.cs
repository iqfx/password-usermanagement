using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace password_usermanagement.Queue
{
    public class RabbitMQListener
    {
        private readonly RabbitMQConnection _connection;

        public RabbitMQListener(RabbitMQConnection connection)
        {
            _connection = connection;
        }

        public void Subscribe<T>(string queueName, string exchangeName, string routingKey, Action<T> handler)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queueName, exchangeName, routingKey);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var jsonMessage = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject<T>(jsonMessage);
                    handler.Invoke(message);
                    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queueName, autoAck: false, consumer);
            }
        }
    }
}
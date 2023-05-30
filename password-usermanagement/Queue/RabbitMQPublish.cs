using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace password_usermanagement.Queue;

public class RabbitMQPublish
{
    private readonly RabbitMQConnection _connection;

    public RabbitMQPublish(RabbitMQConnection connection)
    {
        _connection = connection;
    }

    public void Publish<T>(T message, string exchangeName, string routingKey)
    {
        using (var channel = _connection.CreateModel())
        {
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.BasicPublish(exchangeName, routingKey, properties, body);
        }
    }
}
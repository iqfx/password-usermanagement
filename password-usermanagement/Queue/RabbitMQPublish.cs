using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace password_usermanagement.Queue;

public class RabbitMQPublish: IRabbitMQPublish
{
    private readonly IRabbitMQConnection _connection;

    public RabbitMQPublish(IRabbitMQConnection connection)
    {
        _connection = connection;
    }

    public async Task Publish<T>(T message, string exchangeName, string routingKey, string? concurrencyId)
    {
        using (var channel = _connection.CreateModel())
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var properties = channel.CreateBasicProperties();
            properties.CorrelationId = concurrencyId;
            // if (concurrencyId != null)
            // {
            //     properties.Headers = new Dictionary<string, object>
            //     {
            //         { "concurrency-id", concurrencyId }
            //     };
            // }
            properties.Persistent = true;
            channel.BasicPublish(exchangeName, routingKey, properties, body);
        }
    }
}
namespace password_usermanagement.Queue;

public interface IRabbitMQListener
{
    public Task Subscribe<T>(string queueName, string exchangeName, string routingKey, Action<T> handler);

}
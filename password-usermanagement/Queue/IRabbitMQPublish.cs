namespace password_usermanagement.Queue;

public interface IRabbitMQPublish
{
    public Task Publish<T>(T message, string exchangeName, string routingKey, string? concurrencyId);
}
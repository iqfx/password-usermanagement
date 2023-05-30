namespace password_usermanagement.Queue;

public interface IRabbitMQListener
{
    public void init(string queueName, string exchangeName, string routingKey);

}
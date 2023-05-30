using RabbitMQ.Client;

namespace password_usermanagement.Queue;

public interface IRabbitMQConnection
{
    public IModel CreateModel();
    public void Close();

}
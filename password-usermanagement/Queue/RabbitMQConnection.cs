using RabbitMQ.Client;

namespace password_usermanagement.Queue;

public class RabbitMQConnection : IRabbitMQConnection {
    private readonly string _hostname;
    private readonly string _username;
    private readonly string _password;
    private IConnection _connection;

    public RabbitMQConnection(string hostname, string username, string password)
    {
        _hostname = hostname;
        _username = username;
        _password = password;
    }

    public IModel CreateModel()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };
            _connection = factory.CreateConnection();
        }

        return _connection.CreateModel();
    }

    public void Close()
    {
        _connection?.Close();
    }
}

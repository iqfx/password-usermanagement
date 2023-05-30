using Microsoft.AspNetCore.Mvc;
using password_usermanagement.Models;
using password_usermanagement.Queue;

namespace password_usermanagement.Services;

public class RoleService : IRoleService
{
    private IRabbitMQPublish _publish;
    private IRabbitMQListener _listener;

    public RoleService(IRabbitMQPublish publish, IRabbitMQListener listener)
    {
        _publish = publish;
        _listener = listener;
    }

    private void HandleMessage(string message)
    {
        Console.WriteLine($"Received message: {message}");
        // Perform additional processing
    }
    public async Task<List<Role>> GetAll()
    {
        Action<string> handler = HandleMessage;
        await _listener.Subscribe("random", "test", "jemoeder", handler);

        await _publish.Publish("hallo","test","jemoeder");

        Console.WriteLine("test");
        throw new NotImplementedException();
    }

    public async Task<Role> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> AddRoleToUser(Guid userId, Guid roleId)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        throw new NotImplementedException();
    }
}
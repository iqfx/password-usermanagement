using Microsoft.AspNetCore.Mvc;
using password_usermanagement.Models;
using password_usermanagement.Queue;

namespace password_usermanagement.Services;

public class RoleService : IRoleService
{
    private RabbitMQConnection _connection;

    public RoleService(RabbitMQConnection connection)
    {
        _connection = connection;
    }
    public async Task<List<Role>> GetAll()
    {
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
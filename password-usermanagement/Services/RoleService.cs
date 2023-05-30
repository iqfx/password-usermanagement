using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using password_usermanagement.Data;
using password_usermanagement.Models;
using password_usermanagement.Queue;

namespace password_usermanagement.Services;

public class RoleService : IRoleService
{
    private IRabbitMQPublish _publish;
    private IRabbitMQListener _listener;
    private readonly DatabaseContext _context;

    public RoleService(IRabbitMQPublish publish, IRabbitMQListener listener, DatabaseContext context)
    {
        _publish = publish;
        _listener = listener;
        _context = context;
    }


    public async Task<List<Role>> GetAll()
    {
        _listener.init("random", "test", "jemoeder");
        await _publish.Publish("hallo2","test","jemoeder");

        Console.WriteLine("test");
        return await _context.Roles.ToListAsync();
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
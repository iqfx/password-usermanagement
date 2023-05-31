using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using password_usermanagement.Data;
using password_usermanagement.Models;
using password_usermanagement.Queue;

namespace password_usermanagement.Services;

public class RoleService : IRoleService
{
    private IRabbitMQPublish _publish;
    private readonly DatabaseContext _context;

    public RoleService(IRabbitMQPublish publish, DatabaseContext context)
    {
        _publish = publish;
        _context = context;
    }


    public async Task<List<Role>> GetAll()
    {
        await _publish.Publish("hallo2","test","test2");

        Console.WriteLine("test");
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role> GetById(Guid id)
    {
        return await _context.Roles.FindAsync(id) ?? throw new ArgumentException();;
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
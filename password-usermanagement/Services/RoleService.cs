using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using password_usermanagement.Data;
using password_usermanagement.Models;
using password_usermanagement.Queue;

namespace password_usermanagement.Services;

public class RoleService : IRoleService
{
    private readonly DatabaseContext _context;
    private IUserService _userService;

    public RoleService(DatabaseContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Role> Create(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<List<Role>> GetAll()
    {
        //await _publish.Publish("hallo2","test","test2", null);

        Console.WriteLine("test");
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role> GetById(Guid id)
    {
        return await _context.Roles.FindAsync(id) ?? throw new ArgumentException();;
    }

    public async Task AddRoleToUser(string userId, Guid roleId)
    {
        var role = await GetById(roleId);
        User user = null;
        try
        {
            user = await _userService.GetUserByUserId(userId);
        }
        catch (Exception e)
        {
            user = await _userService.SaveNewUser(userId);
        }
        user.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRoleFromUser(string userId, Guid roleId)
    {
        var role = await GetById(roleId);
        User user = null;
        try
        {
            user = await _userService.GetUserByUserId(userId);
        }
        catch (Exception e)
        {
            user = await _userService.SaveNewUser(userId);
        }
        user.Roles.Remove(role);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Role>> GetRolesFromUser(string userId)
    {
        var user = await _userService.GetUserByUserId(userId);
        return user.Roles.ToList();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using password_usermanagement.Data;
using password_usermanagement.Models;

namespace password_usermanagement.Services;

public class UserService:  IUserService
{
    private DatabaseContext _context;

    public UserService(DatabaseContext context)
    {
        _context = context;
    }
    public async Task<List<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetById(Guid id)
    {
        return await _context.Users.FindAsync(id) ?? throw new ArgumentException();
    }

    public async Task<User> GetUserByUserId(string id)
    {
        return await _context.Users.Where(u => u.userId == id).SingleOrDefaultAsync() ?? throw new ArgumentException();
    }

    public async Task<User> SaveUser(string userId)
    {
        var user = new User()
        {
            userId = userId
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
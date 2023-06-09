using System.IdentityModel.Tokens.Jwt;
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

    public string GetUserIdFromHeader(string header)
    {
        string jwt = header.Replace("Bearer ", string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwt);
        var tokenS = jsonToken as JwtSecurityToken;
        var sub = tokenS.Claims.First(claim => claim.Type == "sub").Value;
        return sub;
    }

    public async Task<User> GetUserByUserId(string id)
    {
        try
        {
            return await _context.Users.Where(u => u.userId == id).Include(r => r.Roles).SingleOrDefaultAsync() ??
                   throw new ArgumentException();
        }
        catch (ArgumentException e)
        {
            var user = await SaveNewUser(id);
            return user;
        }

    }

    public async Task<User> SaveNewUser(string userId)
    {
        var user = new User()
        {
            userId = userId
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> SaveUserSetPasswordSetToTrue(User user)
    {
        var userFromDb = await GetUserByUserId(user.userId);
        userFromDb.HasSetMasterPassword = true;
        await _context.SaveChangesAsync();
        return user;
    }
}
using Microsoft.AspNetCore.Mvc;
using password_usermanagement.Models;

namespace password_usermanagement.Services;

public interface IUserService
{
    public Task<List<User>> GetAll();
    public Task<User> GetById(Guid id);
    public Task<User> GetUserByUserId(string id);
    public Task<User> SaveUser(string userId);
}
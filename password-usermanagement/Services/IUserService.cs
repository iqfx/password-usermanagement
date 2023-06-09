﻿using Microsoft.AspNetCore.Mvc;
using password_usermanagement.Models;
using password_usermanagement.Queue;

namespace password_usermanagement.Services;

public interface IUserService
{
    public Task<List<User>> GetAll();
    public Task<User> GetById(Guid id);
    public Task<User> GetUserByUserId(string id);
    public string GetUserIdFromHeader(string header);
    public Task<User> SaveNewUser(string userId);
    public Task<User> SaveUserSetPasswordSetToTrue(User userId);
    public Task DeleteUser(User userToDelete, User actor);

}
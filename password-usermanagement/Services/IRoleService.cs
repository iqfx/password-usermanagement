using Microsoft.AspNetCore.Mvc;
using password_usermanagement.Models;

namespace password_usermanagement.Services;

public interface IRoleService
{
    public Task<List<Role>> GetAll();
    public Task<Role> GetById(Guid id);
    public Task<Role> Create(Role role);

    public Task AddRoleToUser(string userId, Guid roleId);
    public Task RemoveRoleFromUser(string userId, Guid roleId);
    public Task<List<Role>> GetRolesFromUser(string userId);

}
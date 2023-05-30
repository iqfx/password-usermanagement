using Microsoft.AspNetCore.Mvc;
using password_usermanagement.Models;

namespace password_usermanagement.Services;

public interface IRoleService
{
    public Task<List<Role>> GetAll();
    public Task<Role> GetById(Guid id);
    public Task<IActionResult> AddRoleToUser(Guid userId, Guid roleId);
    public Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId);

}
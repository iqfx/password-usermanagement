using password_usermanagement.Models;

namespace password_usermanagement.Services;

public interface IRoleService
{
    public Task<List<Role>> GetAll();
    public Task<Role> GetById(Guid id);
    public void AddRoleToUser(Guid userId, Guid roleId);
    public void RemoveRoleFromUser(Guid userId, Guid roleId);

}
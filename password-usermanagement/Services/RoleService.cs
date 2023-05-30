using password_usermanagement.Models;

namespace password_usermanagement.Services;

public class RoleService : IRoleService
{
    public async Task<List<Role>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<Role> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public void AddRoleToUser(Guid userId, Guid roleId)
    {
        throw new NotImplementedException();
    }

    public void RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        throw new NotImplementedException();
    }
}
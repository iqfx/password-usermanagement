using AutoMapper;
using password_usermanagement.DTO;
using password_usermanagement.Models;

namespace password_usermanagement.Mappers;

public class RoleMapper: Profile
{
    public RoleMapper()
    {
        CreateMap<Role, RoleDTO>().ReverseMap();

    }
}
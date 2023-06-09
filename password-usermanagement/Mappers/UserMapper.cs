using AutoMapper;
using password_usermanagement.DTO;
using password_usermanagement.Models;

namespace password_usermanagement.Mappers;

public class UserMapper: Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}
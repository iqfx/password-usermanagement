using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using password_usermanagement.DTO;
using password_usermanagement.Models;
using password_usermanagement.Services;

namespace password_usermanagement.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private IMapper _mapper;
    private IUserService _userService;

    public UserController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }
    [HttpGet("{id}")]
    public async Task<UserDto> GetUserMasterPasswordBool(string id)
    {
        return _mapper.Map<UserDto>(await _userService.GetUserByUserId(id));
    }

    [HttpGet]
    public async Task<UserDto> GetUserMasterPasswordBool()
    {
        var userId = _userService.GetUserIdFromHeader(Request.Headers.Authorization);
        return _mapper.Map<UserDto>(await _userService.GetUserByUserId(userId));
    }

    [HttpPost]
    public async Task<IActionResult> SetUserMasterPasswordToTrue()
    {
        var user = await _userService.GetUserByUserId(_userService.GetUserIdFromHeader(Request.Headers.Authorization));
        await _userService.SaveUserSetPasswordSetToTrue(_mapper.Map<User>(user));
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userService.GetUserByUserId(id);

        var userActor = await _userService.GetUserByUserId(_userService.GetUserIdFromHeader(Request.Headers.Authorization));
        await _userService.DeleteUser(user, userActor);
        return Ok();
    }
}
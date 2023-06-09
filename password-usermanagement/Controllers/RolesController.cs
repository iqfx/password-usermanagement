﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using password_usermanagement.DTO;
using password_usermanagement.Models;
using password_usermanagement.Services;

namespace password_usermanagement.Controllers;
[ApiController]
[Route("[controller]")]
public class RolesController : ControllerBase
{
    private RoleService _roleService;
    private IMapper _mapper;

    public RolesController(RoleService roleService, IMapper mapper)
    {
        _roleService = roleService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<List<RoleDTO>> GetAllRoles()
    {
        return _mapper.Map<List<RoleDTO>>(await _roleService.GetAll());
    }
    [HttpGet("{id}")]
    public async Task<RoleDTO> GetRoleById(Guid id)
    {
        return _mapper.Map<RoleDTO>(await _roleService.GetById(id));
    }

    [HttpPost("{roleId}/{userId}")]
    public async Task<IActionResult> AddRoleToUser(Guid roleId, string userId)
    {
        await _roleService.AddRoleToUser(userId, roleId);
        return NoContent();
    }

    [HttpDelete("{roleId}/{userId}")]
    public async Task<IActionResult> Remove(Guid roleId, string userId)
    {
        await _roleService.RemoveRoleFromUser(userId, roleId);
        return NoContent();
    }

    [HttpPost]
    public async Task<RoleDTO> Create(RoleDTO role)
    {
        var roleModel = _mapper.Map<Role>(role);
        return _mapper.Map<RoleDTO>(await _roleService.Create(roleModel));
    }
}
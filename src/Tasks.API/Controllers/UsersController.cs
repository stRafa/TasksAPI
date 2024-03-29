﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tasks.Application.DTOs.User;
using Tasks.Application.Interfaces;

namespace Tasks.API.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUser()
    {
        var result = await _userService.GetAll();

        if (!result.Success)
            return StatusCode(StatusCodes.Status403Forbidden, result);
        

        return Ok(await _userService.GetAll());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetById(id);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserInfo(EditUserDTO model)
    {
        var result = await _userService.UpdateUserInfo(model);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.Delete(id);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return NotFound(result);
        }

        return Ok(result);
    }
}
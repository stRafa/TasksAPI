using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tasks.Application;
using Tasks.Application.DTOs.User;
using Tasks.Application.Interfaces;

namespace Tasks.API.Controllers;

[Authorize]
[ApiController]
[Route("api/missions")]
public class MissionsController : ControllerBase
{
    private readonly IUserService _userService;

    public MissionsController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMission(CreateMissionDTO model)
    {
        var result = await _userService.AddMission(model);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("position")]
    public async Task<IActionResult> UpdateMissionPosition(EditMissionsPositionDTO model)
    {
        var result = await _userService.UpdateMissionsPosition(model);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return BadRequest(result);
        }

        return Ok(result);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateMission(EditMissionDTO model)
    {
        var result = await _userService.UpdateMission(model);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMission(Guid id)
    {
        var result = await _userService.DeleteMission(id);

        if (!result.Success)
        {
            if (HttpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return BadRequest(result);
        }

        return Ok(result);
    }
}
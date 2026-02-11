
using CRM_Backend.DTOs.Users;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM_Backend.Controllers;

[ApiController]
[Route("api/users")]

[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]

public class UsersController : ControllerBase
{
    private readonly IUserManagementService _users;

    public UsersController(IUserManagementService users)
    {
        _users = users;
    }

    // CREATE USER
    [HttpPost]
    [HasPermission("USER_CREATE")]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var actorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userId = await _users.CreateUserAsync(dto, actorId);
        return Ok(new { userId });
    }

    // UPDATE USER
    [HttpPut("{userId:long}")]
    [HasPermission("USER_UPDATE")]
    public async Task<IActionResult> UpdateUser(long userId, UpdateUserDto dto)
    {
        var actorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _users.UpdateUserAsync(userId, dto, actorId);
        return Ok();
    }

    // ASSIGN MANAGER
    [HttpPut("{userId:long}/manager")]
    [HasPermission("USER_ASSIGN_MANAGER")]
    public async Task<IActionResult> AssignManager(long userId, AssignManagerDto dto)
    {
        await _users.AssignManagerAsync(userId, dto.ManagerId);
        return Ok();
    }

    // ADMIN USER LIST
    [HttpGet]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetAdminUserList(
        [FromQuery] string domainCode,
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] string? roleCode)
    {
        var users = await _users.GetAdminUsersByDomainAsync(
            domainCode, search, status, roleCode);
        return Ok(users);
    }

    // MANAGERS DROPDOWN
    [HttpGet("managers")]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetManagers([FromQuery] string domainCode)
    {
        var managers = await _users.GetManagersByDomainAsync(domainCode);
        return Ok(managers);
    }

    // GLOBAL MANAGERS (ADMIN)
    [HttpGet("admin/managers")]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetAllManagers()
    {
        return Ok(await _users.GetAllManagersAsync());
    }

    // GET USER DETAILS
    [HttpGet("{userId:long}")]
    [HasPermission("USER_VIEW")]
    public async Task<IActionResult> GetUser(long userId)
    {
        return Ok(await _users.GetUserDetailsAsync(userId));
    }

    // MY TEAM
    [HttpGet("me/team")]
    [HasPermission("USER_VIEW_TEAM")]
    public async Task<IActionResult> GetMyTeam()
    {
        var managerId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(await _users.GetTeamByManagerAsync(managerId));
    }

    // LOCK USER
    [HttpPut("{userId:long}/lock")]
    [HasPermission("USER_LOCK")]
    public async Task<IActionResult> LockUser(long userId, LockUserDto dto)
    {
        var actorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _users.LockUserAsync(userId, dto.Reason, actorId);
        return Ok();
    }

    // UNLOCK USER
    [HttpPut("{userId:long}/unlock")]
    [HasPermission("USER_UNLOCK")]
    public async Task<IActionResult> UnlockUser(long userId)
    {
        var actorId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _users.UnlockUserAsync(userId, actorId);
        return Ok();
    }
}

using CRM_Backend.Data;
using CRM_Backend.Exceptions;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


public class UserVisibilityService : IUserVisibilityService
{
    private readonly CrmAuthDbContext _context;

    public UserVisibilityService(CrmAuthDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<long>> GetVisibleUserIdsAsync(long viewerUserId)
    {
        if (viewerUserId <= 0)
            throw new ValidationException("Invalid viewer user id.");

        var viewerExists = await _context.Users
            .AnyAsync(u => u.UserId == viewerUserId);

        if (!viewerExists)
            throw new NotFoundException($"User with id {viewerUserId} not found.");

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == viewerUserId)
            .Select(ur => ur.Role.RoleCode)
            .ToListAsync();

        if (roles == null || !roles.Any())
            throw new ForbiddenException("User has no assigned roles.");

        // ADMIN / HR → everyone
        if (roles.Contains("ADMIN") || roles.Contains("HR"))
        {
            return await _context.Users
                .Select(u => u.UserId)
                .ToListAsync();
        }

        // MANAGER → self + hierarchy
        if (roles.Contains("MANAGER"))
        {
            return await GetHierarchyAsync(viewerUserId);
        }

        // EMPLOYEE → self only
        if (roles.Contains("EMPLOYEE"))
        {
            return new List<long> { viewerUserId };
        }

        throw new ForbiddenException("User role does not allow visibility access.");
    }

    private async Task<List<long>> GetHierarchyAsync(long managerId)
    {
        var managerExists = await _context.Users
            .AnyAsync(u => u.UserId == managerId);

        if (!managerExists)
            throw new NotFoundException($"Manager with id {managerId} not found.");

        var result = new List<long> { managerId };

        async Task Traverse(long id)
        {
            var children = await _context.Users
                .Where(u => u.ManagerId == id)
                .Select(u => u.UserId)
                .ToListAsync();

            foreach (var child in children)
            {
                if (!result.Contains(child))
                {
                    result.Add(child);
                    await Traverse(child);
                }
            }
        }

        await Traverse(managerId);
        return result;
    }
}

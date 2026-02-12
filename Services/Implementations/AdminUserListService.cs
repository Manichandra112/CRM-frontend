using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations;

public class AdminUserListService : IAdminUserListService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserListService(CrmAuthDbContext context)
    {
        _context = context;
    }

    //public async Task<AdminUserListResponseDto> GetUsersAsync(
    // int page,
    // int pageSize)
    //{
    //    var query = _context.Users
    //        .Where(u => u.DeletedAt == null);

    //    var total = await query.CountAsync();

    //    var users = await query
    //        .OrderBy(u => u.Username)
    //        .Skip((page - 1) * pageSize)
    //        .Take(pageSize)
    //        .Select(u => new AdminUserListItemDto
    //        {
    //            UserId = u.UserId,
    //            Name = u.Profile.FirstName + " " + u.Profile.LastName,
    //            Username = u.Username,
    //            Email = u.Email,
    //            Department = u.Department,
    //            Designation = u.Designation,
    //            AccountStatus = u.AccountStatus,
    //            ManagerName = u.Manager != null
    //                ? u.Manager.Profile.FirstName + " " + u.Manager.Profile.LastName
    //                : null,
    //            Roles = u.UserRoles
    //                .Select(r => r.Role.RoleName)
    //                .ToList(),
    //            CreatedAt = u.CreatedAt,
    //            LastActivityAt = u.LastActivityAt
    //        })
    //        .ToListAsync();

    //    return new AdminUserListResponseDto
    //    {
    //        Total = total,
    //        Page = page,
    //        PageSize = pageSize,
    //        Users = users
    //    };
    //}

    public async Task<AdminUserListResponseDto> GetUsersAsync(
    int page,
    int pageSize)
    {
        // Defensive pagination
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        // Cap maximum page size
        if (pageSize > 100)
            pageSize = 100;

        var query = _context.Users
            .AsNoTracking()
            .Where(u => u.DeletedAt == null);

        var total = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserListItemDto
            {
                UserId = u.UserId,

                Name = u.Profile != null
                    ? (u.Profile.FirstName ?? "") + " " +
                      (u.Profile.LastName ?? "")
                    : null,

                Username = u.Username,
                Email = u.Email,
                Department = u.Department,
                Designation = u.Designation,
                AccountStatus = u.AccountStatus,

                ManagerName = u.Manager != null && u.Manager.Profile != null
                    ? (u.Manager.Profile.FirstName ?? "") + " " +
                      (u.Manager.Profile.LastName ?? "")
                    : null,

                Roles = u.UserRoles
                    .Select(r => r.Role.RoleName)
                    .ToList(),

                CreatedAt = u.CreatedAt,
                LastActivityAt = u.LastActivityAt
            })
            .ToListAsync();

        return new AdminUserListResponseDto
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Users = users
        };
    }



}

//using CRM_Backend.Data;
//using CRM_Backend.DTOs.Users;
//using CRM_Backend.Services.Interfaces;
//using Microsoft.EntityFrameworkCore;

//namespace CRM_Backend.Services.Implementations;

//public class AdminUserAuditLogService : IAdminUserAuditLogService
//{
//    private readonly CrmAuthDbContext _context;

//    public AdminUserAuditLogService(CrmAuthDbContext context)
//    {
//        _context = context;
//    }

//    public async Task<List<AdminUserAuditLogDto>> GetUserAuditLogsAsync(long userId)
//    {
//        return await _context.AuditLogs
//            .Where(a => a.TargetUserId == userId)
//            .OrderByDescending(a => a.CreatedAt)
//            .Select(a => new AdminUserAuditLogDto
//            {
//                Action = a.Action,
//                Module = a.Module,
//                Metadata = a.Metadata,

//                ActorName = a.ActorUserId == null
//                    ? "SYSTEM"
//                    : _context.Users
//                        .Where(u => u.UserId == a.ActorUserId)
//                        .Select(u => u.Username)
//                        .FirstOrDefault(),

//                CreatedAt = a.CreatedAt
//            })
//            .ToListAsync();
//    }
//}
using CRM_Backend.Data;
using CRM_Backend.DTOs.Users;
using CRM_Backend.Exceptions;
using CRM_Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM_Backend.Services.Implementations;

public class AdminUserAuditLogService : IAdminUserAuditLogService
{
    private readonly CrmAuthDbContext _context;

    public AdminUserAuditLogService(CrmAuthDbContext context)
    {
        _context = context
            ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<AdminUserAuditLogDto>> GetUserAuditLogsAsync(long userId)
    {
        // 1️⃣ Validate input
        if (userId <= 0)
            throw new ValidationException("Invalid user id.");

        // 2️⃣ Ensure user exists
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserId == userId && u.DeletedAt == null);

        if (!userExists)
            throw new NotFoundException($"User {userId} not found.");

        // 3️⃣ Proper LEFT JOIN (no navigation property required)
        var logs = await (
            from a in _context.AuditLogs.AsNoTracking()
            join u in _context.Users.AsNoTracking()
                on a.ActorUserId equals u.UserId into actorGroup
            from actor in actorGroup.DefaultIfEmpty()
            where a.TargetUserId == userId
            orderby a.CreatedAt descending
            select new AdminUserAuditLogDto
            {
                Action = a.Action,
                Module = a.Module,
                Metadata = a.Metadata,
                ActorName = actor != null ? actor.Username : "SYSTEM",
                CreatedAt = a.CreatedAt
            }
        ).ToListAsync();

        return logs;
    }
}


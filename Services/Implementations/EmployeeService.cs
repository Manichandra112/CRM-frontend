using CRM_Backend.Data;
using CRM_Backend.DTOs.Common;
using CRM_Backend.DTOs.Employees;
using Microsoft.EntityFrameworkCore;

public class EmployeeService : IEmployeeService
{
    private readonly CrmAuthDbContext _context;

    public EmployeeService(CrmAuthDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EmployeeDto>> GetVisibleEmployeesAsync(
     long currentUserId,
     IEnumerable<string>? roles,
     IEnumerable<string>? permissions,
     EmployeeFilterDto filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        roles ??= Enumerable.Empty<string>();
        permissions ??= Enumerable.Empty<string>();

        var normalizedPermissions = permissions
            .Select(p => p.ToUpper())
            .ToHashSet();

        var normalizedRoles = roles
            .Select(r => r.ToUpper())
            .ToList();

        var query = _context.Users
            .AsNoTracking()
            .AsQueryable();

        // 🔐 VISIBILITY RULES
        if (normalizedPermissions.Contains("CRM_FULL_ACCESS"))
        {
            // Full visibility
        }
        else if (normalizedRoles.Any(r => r.EndsWith("_MANAGER")))
        {
            query = query.Where(u =>
                u.UserId == currentUserId ||
                u.ManagerId == currentUserId);
        }
        else
        {
            query = query.Where(u => u.UserId == currentUserId);
        }

        // 🌍 DOMAIN FILTER
        if (!string.IsNullOrWhiteSpace(filter.DomainCode))
        {
            var domainCode = filter.DomainCode.Trim().ToUpper();
            query = query.Where(u =>
                u.Domain.DomainCode.ToUpper() == domainCode);
        }

        // 👤 MANAGER FILTER
        if (filter.ManagerId.HasValue)
        {
            query = query.Where(u =>
                u.ManagerId == filter.ManagerId.Value);
        }

        // 🔍 SEARCH (case-insensitive for PostgreSQL)
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = $"%{filter.Search.Trim()}%";

            query = query.Where(u =>
                EF.Functions.ILike(u.Username, search) ||
                EF.Functions.ILike(u.Email, search));
        }

        // 📄 Pagination Safety
        var page = filter.Page <= 0 ? 1 : filter.Page;
        var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new EmployeeDto
            {
                UserId = u.UserId,
                Username = u.Username,
                Department = u.Department,
                Designation = u.Designation,
                ManagerId = u.ManagerId,
                DomainId = u.DomainId
            })
            .ToListAsync();

        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

}

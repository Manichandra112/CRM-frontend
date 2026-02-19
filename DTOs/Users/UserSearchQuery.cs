namespace CRM_Backend.DTOs.Users;

public class UserSearchQuery
{
    // -----------------------------
    // Filtering
    // -----------------------------
    public string? DomainCode { get; set; }
    public string? RoleCode { get; set; }
    public string? Status { get; set; }
    public string? Search { get; set; }

    // -----------------------------
    // Pagination
    // -----------------------------
    private const int MaxPageSize = 100;

    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize
            ? MaxPageSize
            : (value < 1 ? 10 : value);
    }

    // -----------------------------
    // Sorting (optional but smart)
    // -----------------------------
    public string? SortBy { get; set; } = "Username";
    public string? SortDirection { get; set; } = "asc";
}

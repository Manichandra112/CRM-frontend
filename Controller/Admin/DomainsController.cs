
using CRM_Backend.DTOs.Domains;
using CRM_Backend.Security.Authorization;
using CRM_Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Backend.Controller.Admin;

/// <summary>
/// Provides administrative operations for managing system domains.
/// Domains are used to logically group permissions within the CRM system.
/// </summary>
/// <remarks>
/// Access Requirements:
/// - Authenticated user
/// - ACCOUNT_ACTIVE policy
/// - PASSWORD_RESET_COMPLETED policy
/// - Specific DOMAIN_* permission depending on the operation
/// </remarks>
[ApiController]
[Route("api/admin/domains")]
[Authorize(Policy = "ACCOUNT_ACTIVE")]
[Authorize(Policy = "PASSWORD_RESET_COMPLETED")]
[Produces("application/json")]
public class DomainsController : ControllerBase
{
    private readonly IDomainService _domains;

    public DomainsController(IDomainService domains)
    {
        _domains = domains;
    }

    /// <summary>
    /// Creates a new domain.
    /// </summary>
    /// <remarks>
    /// Permission Required: DOMAIN_CREATE
    ///
    /// Sample Request:
    ///
    ///     POST /api/admin/domains
    ///     {
    ///         "domainCode": "HR",
    ///         "domainName": "Human Resources"
    ///     }
    /// </remarks>
    /// <param name="dto">Domain creation payload.</param>
    /// <response code="200">Domain successfully created.</response>
    /// <response code="400">Validation error or invalid input.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks DOMAIN_CREATE permission.</response>
    [HttpPost]
    [HasPermission("DOMAIN_CREATE")]
    [ProducesResponseType(typeof(DomainResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateDomainDto dto)
    {
        var created = await _domains.CreateAsync(dto);
        return Ok(created);
    }

    /// <summary>
    /// Retrieves all domains.
    /// </summary>
    /// <remarks>
    /// Permission Required: DOMAIN_VIEW
    /// </remarks>
    /// <response code="200">Returns list of domains.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks DOMAIN_VIEW permission.</response>
    [HttpGet]
    [HasPermission("DOMAIN_VIEW")]
    [ProducesResponseType(typeof(IEnumerable<DomainResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll()
    {
        var domains = await _domains.GetAllAsync();
        return Ok(domains);
    }

    /// <summary>
    /// Updates the active status of a domain.
    /// </summary>
    /// <remarks>
    /// Permission Required: DOMAIN_UPDATE
    ///
    /// Only the Active status of the domain can be modified.
    ///
    /// Sample Request:
    ///
    ///     PUT /api/admin/domains/5
    ///     {
    ///         "isActive": false
    ///     }
    /// </remarks>
    /// <param name="id">Unique identifier of the domain.</param>
    /// <param name="dto">Domain status update payload.</param>
    /// <response code="200">Domain successfully updated.</response>
    /// <response code="400">Invalid input.</response>
    /// <response code="404">Domain not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User lacks DOMAIN_UPDATE permission.</response>
    [HttpPut("{id:long}")]
    [HasPermission("DOMAIN_UPDATE")]
    [ProducesResponseType(typeof(DomainResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDomainDto dto)
    {
        var updated = await _domains.UpdateAsync(id, dto);
        return Ok(updated);
    }
}

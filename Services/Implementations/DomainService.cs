using CRM_Backend.DTOs.Domains;
using CRM_Backend.Exceptions;
using CRM_Backend.Repositories.Interfaces;
using CRM_Backend.Services.Interfaces;
using DomainEntity = CRM_Backend.Domain.Entities.Domain;

namespace CRM_Backend.Services.Implementations
{
    public class DomainService : IDomainService
    {
        private readonly IDomainRepository _domains;

        public DomainService(IDomainRepository domains)
        {
            _domains = domains
                ?? throw new ArgumentNullException(nameof(domains));
        }

        private static DomainResponseDto Map(DomainEntity domain)
        {
            return new DomainResponseDto
            {
                DomainId = domain.DomainId,
                DomainCode = domain.DomainCode,
                DomainName = domain.DomainName,
                Active = domain.Active,
                CreatedAt = domain.CreatedAt,
                UpdatedAt = domain.UpdatedAt
            };
        }

        public async Task<DomainResponseDto> CreateAsync(CreateDomainDto dto)
        {
            if (dto == null)
                throw new ValidationException("Domain data is required.");

            if (string.IsNullOrWhiteSpace(dto.DomainCode))
                throw new ValidationException("DomainCode is required.");

            if (string.IsNullOrWhiteSpace(dto.DomainName))
                throw new ValidationException("DomainName is required.");

            var normalizedCode = dto.DomainCode.Trim().ToUpper();
            var normalizedName = dto.DomainName.Trim();

            var existing = await _domains.GetByCodeAsync(normalizedCode);
            if (existing != null)
                throw new ConflictException(
                    $"Domain '{normalizedCode}' already exists.");

            var domain = await _domains.CreateAsync(
                normalizedCode,
                normalizedName);

            return Map(domain);
        }

        public async Task<List<DomainResponseDto>> GetAllAsync()
        {
            var domains = await _domains.GetAllAsync();
            return domains.Select(Map).ToList();
        }

        public async Task<DomainResponseDto> UpdateAsync(
            long id,
            UpdateDomainDto dto)
        {
            if (id <= 0)
                throw new ValidationException("Invalid domain id.");

            if (dto == null)
                throw new ValidationException("Update data is required.");

            var domain = await _domains.GetByIdAsync(id)
                ?? throw new NotFoundException($"Domain {id} not found.");

            // Optional business rule:
            if (domain.DomainCode == "SYSTEM")
                throw new BusinessRuleException(
                    "SYSTEM domain cannot be modified.");

            domain.Active = dto.IsActive;
            domain.UpdatedAt = DateTime.UtcNow;

            await _domains.UpdateAsync(domain);

            return Map(domain);
        }
    }
}

namespace CRM_Backend.Services.Implementations
{
    using CRM_Backend.Data;
    using CRM_Backend.Domain.Entities;
    using CRM_Backend.Exceptions;
    using CRM_Backend.Services.Interfaces;
    using System.Text.Json;

    public class AuditLogService : IAuditLogService
    {
        private readonly CrmAuthDbContext _context;

        public AuditLogService(CrmAuthDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task LogAsync(
            long? actorUserId,
            long? targetUserId,
            string action,
            string module,
            object? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ValidationException("Action is required.");

            if (string.IsNullOrWhiteSpace(module))
                throw new ValidationException("Module is required.");

            string? serializedMetadata = null;

            if (metadata != null)
            {
                serializedMetadata = JsonSerializer.Serialize(
                    metadata,
                    new JsonSerializerOptions
                    {
                        WriteIndented = false
                    });

                // Optional safety guard (avoid huge blobs)
                if (serializedMetadata.Length > 5000)
                {
                    serializedMetadata =
                        serializedMetadata.Substring(0, 5000);
                }
            }

            var audit = new AuditLog
            {
                ActorUserId = actorUserId,
                TargetUserId = targetUserId,
                Action = action.Trim().ToUpper(),
                Module = module.Trim().ToUpper(),
                Metadata = serializedMetadata,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                _context.AuditLogs.Add(audit);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Logging must never crash the system.
                // Swallow or optionally log to fallback system.
            }
        }
    }
}

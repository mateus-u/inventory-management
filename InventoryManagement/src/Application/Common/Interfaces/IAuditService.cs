using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface IAuditService
{
    Task<AuditLogResponseDto> LogActionAsync(AuditLogDto auditLog, CancellationToken cancellationToken = default);
}

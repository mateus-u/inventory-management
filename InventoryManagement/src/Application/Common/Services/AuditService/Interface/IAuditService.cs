using Application.Common.Services.AuditService.Requests;
using Application.Common.Services.AuditService.Responses;

namespace Application.Common.Services.AuditService.Interface;

public interface IAuditService
{
    Task<AuditLogResponse> LogActionAsync(AuditLogRequest auditLog, CancellationToken cancellationToken = default);
}

namespace Application.Common.Services.AuditService.Requests;

public record AuditLogRequest(
    string UserId,
    string Email,
    string ActionName,
    DateTime Timestamp
);

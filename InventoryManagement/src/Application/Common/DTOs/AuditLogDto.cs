namespace Application.Common.DTOs;

public record AuditLogDto(
    string UserId,
    string Email,
    string ActionName,
    DateTime Timestamp
);
